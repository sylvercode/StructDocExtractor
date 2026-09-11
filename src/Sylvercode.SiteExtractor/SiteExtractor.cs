using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

/// <summary>Main <see cref="ISiteExtractor"/> implementation that orchestrates site extraction through a two-phase resource processing pipeline.</summary>
/// <remarks>
/// Accepts a seed URI, resolves an <see cref="IResourceProcessor"/> for it via <see cref="IResourceProcessorProvider"/>,
/// then drives a queue-based loop: each dequeued resource is processed, its dependencies enqueued, and any metadata or
/// URI translaters returned by the processor are applied. Resources that report <c>IsUnfinished</c> are collected and
/// retried in subsequent batches until all resources are fully extracted.
/// </remarks>
public partial class SiteExtractor(
    IResourceProcessorProvider processorProvider,
    ILogger<SiteExtractor>? logger = null) : ISiteExtractor
{
    private readonly ResourcesTracker _resourcesTracker = new(processorProvider);

    private readonly ILogger<SiteExtractor> _logger = logger ?? NullLogger<SiteExtractor>.Instance;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// No <see cref="IResourceProcessor"/> is registered for <paramref name="uri"/>, or a continuation
    /// pass attempts to introduce new resource dependencies.
    /// </exception>
    public void Extract(Uri uri)
    {
        _resourcesTracker.AddResource(uri, out bool hasResourceProcessor);
        if (!hasResourceProcessor)
            throw new InvalidOperationException("No resource processor found for the given URI.");

        List<IResourceProcessorResult> unfinishedProcesses = [];

        while (_resourcesTracker.ResourceQueue.HasQueuedResources)
        {
            var (resource, processor) = _resourcesTracker.ResourceQueue.Dequeue();

            KeyValuePair<string, object?>[] pullScopeState =
            [
                new("Uri", resource.Uri),
                new("Processor", processor.GetType().Name),
            ];
            using var logScope = _logger.BeginScope(pullScopeState);

            LogPullingResource();
            resource.MarkAsPulling();

            IResourceProcessorResult result = processor.Process(resource, _resourcesTracker.Resources);

            foreach (Uri dependency in result.GetResourceDependencies())
            {
                LogAddDependency(dependency);
                _resourcesTracker.AddResource(dependency);
            }

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

            if (result.ResourceUriTranslaterToSet != null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    string uriTranslaterType = result.ResourceUriTranslaterToSet.GetType().Name;
                    LogUriTranslaterToSet(uriTranslaterType);
                }

                resource.UriTranslater = result.ResourceUriTranslaterToSet;
            }

            if (result.IsUnfinished)
            {
                LogUnfinishedProcess();
                unfinishedProcesses.Add(result);
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
            List<IResourceProcessorResult> unfinishedProcessesNext = [];

            foreach (IResourceProcessorResult result in unfinishedProcesses)
            {
                KeyValuePair<string, object?>[] continueScopeState =
                [
                    new("Uri", result.Resource.Uri),
                    new("Processor", result.Processor.GetType().Name),
                ];
                using var logScope = _logger.BeginScope(continueScopeState);

                LogContinueProcess();

                IResourceProcessorResult nextResult = result.ContinueProcess();

                if (nextResult.GetResourceDependencies().Any())
                    throw new InvalidOperationException("Resource dependencies are not allowed in the continue process.");

                if (nextResult.IsUnfinished)
                {
                    LogUnfinishedProcess();
                    unfinishedProcessesNext.Add(nextResult);
                }
                else
                {
                    LogRessourcePulled();
                    result.Resource.MarkAsPulled();
                }
            }

            unfinishedProcesses = unfinishedProcessesNext;
        }
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Pulling resource")]
    private partial void LogPullingResource();

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
