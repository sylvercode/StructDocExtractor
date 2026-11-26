using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

/// <summary>Main <see cref="ISiteExtractor"/> implementation that orchestrates site extraction through a two-phase resource processing pipeline.</summary>
/// <remarks>
/// Accepts a seed URI, resolves an <see cref="IResourceProcessor"/> for it via <see cref="IResourceProcessorProvider"/>,
/// then drives a queue-based loop: each dequeued resource is processed, its dependencies enqueued, and any metadata or
/// URI translaters returned by the processor are applied. Resources that report <c>IsUnfinished</c> are collected and
/// retried in subsequent batches until all resources are fully extracted. When an
/// <see cref="IExtractionManifestCatalog"/> is registered, it owns manifest validation and persistence while this
/// type marks the beginning and successful completion of the extraction.
/// </remarks>
public partial class SiteExtractor(
    IResourceProcessorProvider processorProvider,
    IExtractionManifestCatalog? manifestCatalog = null,
    ILogger<SiteExtractor>? logger = null) : ISiteExtractor
{
    private readonly ILogger<SiteExtractor> _logger = logger ?? NullLogger<SiteExtractor>.Instance;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// No <see cref="IResourceProcessor"/> is registered for <paramref name="uri"/>, or a continuation
    /// pass attempts to introduce new resource dependencies.
    /// </exception>
    public void Extract(Uri uri)
    {
        manifestCatalog?.BeginExtraction(uri);
        var resourcesTracker = new ResourcesTracker(processorProvider);

        resourcesTracker.AddResource(uri, out bool hasResourceProcessor);
        if (!hasResourceProcessor)
            throw new InvalidOperationException("No resource processor found for the given URI.");

        List<(Resource Resource, IResourceProcessorResult Result)> unfinishedProcesses = [];

        while (resourcesTracker.ResourceQueue.HasQueuedResources)
        {
            var (resource, processor) = resourcesTracker.ResourceQueue.Dequeue();

            KeyValuePair<string, object?>[] pullScopeState =
            [
                new("Uri", resource.Uri),
                new("Processor", processor.GetType().Name),
            ];
            using var logScope = _logger.BeginScope(pullScopeState);

            LogPullingResource();
            resource.MarkAsPulling();

            IResourceProcessorResult result = Process(processor, resource, resourcesTracker.Resources);

            foreach (Uri dependency in result.GetResourceDependencies())
            {
                LogAddDependency(dependency);
                resourcesTracker.AddResource(dependency);
            }

            ApplyResultChanges(resource, result);

            if (result.IsUnfinished)
            {
                LogUnfinishedProcess();
                unfinishedProcesses.Add((resource, result));
            }
            else
            {
                LogRessourcePulled();
                resource.MarkAsPulled();
            }
        }

        while (unfinishedProcesses.Count != 0)
        {
            LogBeggingUnfinishProcessBatch();
            List<(Resource Resource, IResourceProcessorResult Result)> unfinishedProcessesNext = [];

            foreach ((Resource resource, IResourceProcessorResult result) in unfinishedProcesses)
            {
                KeyValuePair<string, object?>[] continueScopeState =
                [
                    new("Uri", resource.Uri),
                    new("Processor", result.Processor.GetType().Name),
                ];
                using var logScope = _logger.BeginScope(continueScopeState);

                LogContinueProcess();

                IResourceProcessorResult nextResult = ContinueProcess(resource, result);

                if (nextResult.GetResourceDependencies().Any())
                    throw new InvalidOperationException("Resource dependencies are not allowed in the continue process.");

                ApplyResultChanges(resource, nextResult);

                if (nextResult.IsUnfinished)
                {
                    LogUnfinishedProcess();
                    unfinishedProcessesNext.Add((resource, nextResult));
                }
                else
                {
                    LogRessourcePulled();
                    resource.MarkAsPulled();
                }
            }

            unfinishedProcesses = unfinishedProcessesNext;
        }

        manifestCatalog?.CompleteExtraction(resourcesTracker.Resources);
    }

    private IResourceProcessorResult Process(
        IResourceProcessor processor,
        Resource resource,
        IReadOnlyResourceRepository resources)
    {
        try
        {
            return processor.Process(resource, resources);
        }
        catch (Exception exception)
        {
            resource.MarkAsFailed();
            LogResourceProcessFailed(exception, resource.Uri, nameof(Process));
            throw;
        }
    }

    private IResourceProcessorResult ContinueProcess(
        Resource resource,
        IResourceProcessorResult result)
    {
        try
        {
            return result.ContinueProcess(resource);
        }
        catch (Exception exception)
        {
            resource.MarkAsFailed();
            LogResourceProcessFailed(exception, resource.Uri, nameof(ContinueProcess));
            throw;
        }
    }

    private void ApplyResultChanges(Resource resource, IResourceProcessorResult result)
    {
        if (result.NewMetadata.Count != 0)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                foreach (var (name, metadata) in result.NewMetadata)
                {
                    string? metadataValue = metadata.GetStrValue();
                    LogNewMetadata(name, metadataValue);
                }
            }

            resource.Metadata.CopyMetadataFrom(result.NewMetadata);
        }

        if (result.ResourceUriTranslaterToSet is not null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                string uriTranslaterType = result.ResourceUriTranslaterToSet.GetType().Name;
                LogUriTranslaterToSet(uriTranslaterType);
            }

            resource.SetUriTranslater(result.ResourceUriTranslaterToSet);
        }

        if (!result.IsUnfinished)
            resource.MarkExtractionState(result.ExtractionState);
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Pulling resource")]
    private partial void LogPullingResource();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Resource {Uri} failed during {Operation}.")]
    private partial void LogResourceProcessFailed(Exception exception, Uri uri, string operation);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Adding dependency: {Dependency}")]
    private partial void LogAddDependency(Uri dependency);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "New metadata: {Name} = {Value}")]
    private partial void LogNewMetadata(string name, string? value);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Setting UriTranslater: {UriTranslater}")]
    private partial void LogUriTranslaterToSet(string uriTranslater);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Unfinished process.")]
    private partial void LogUnfinishedProcess();

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Resource pulled.")]
    private partial void LogRessourcePulled();

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Begging unfinished process batch.")]
    private partial void LogBeggingUnfinishProcessBatch();

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Continue process.")]
    private partial void LogContinueProcess();
}
