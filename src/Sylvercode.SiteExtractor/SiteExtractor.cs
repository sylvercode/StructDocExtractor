using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

/// <summary>Main <see cref="ISiteExtractor"/> implementation that orchestrates site extraction through a two-phase resource processing pipeline.</summary>
/// <remarks>
/// Accepts a seed URI, resolves an <see cref="IResourceProcessor"/> for it via <see cref="IResourceProcessorProvider"/>,
/// then drives a queue-based loop: each dequeued resource is processed, its dependencies enqueued, and any metadata or
/// URI translaters returned by the processor are applied. Resources that report <c>IsUnfinished</c> are collected and
/// retried in subsequent batches until all resources are fully extracted. Extraction observers receive lifecycle
/// and result events to apply concerns such as metadata, URI translation, and manifest persistence.
/// </remarks>
public partial class SiteExtractor(
    IResourceProcessorProvider processorProvider,
    IEnumerable<IExtractionObserver> extractionObservers,
    ILogger<SiteExtractor>? logger = null) : ISiteExtractor
{
    private readonly IExtractionObserver[] _extractionObservers = [.. extractionObservers];

    private readonly ILogger<SiteExtractor> _logger = logger ?? NullLogger<SiteExtractor>.Instance;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// No <see cref="IResourceProcessor"/> is registered for <paramref name="uri"/>, or a continuation
    /// pass attempts to introduce new resource dependencies.
    /// </exception>
    public void Extract(Uri uri)
    {
        Resource? activeResource = null;
        string operation = nameof(Extract);
        try
        {
            Notify(new ExtractionStarted(uri));
            var resourcesTracker = new ResourcesTracker(processorProvider);

            resourcesTracker.AddResource(uri, out bool hasResourceProcessor);
            if (!hasResourceProcessor)
                throw new InvalidOperationException("No resource processor found for the given URI.");

            List<PendingProcess> unfinishedProcesses = [];

            while (resourcesTracker.ResourceQueue.HasQueuedResources)
            {
                var (resource, processor) = resourcesTracker.ResourceQueue.Dequeue();
                activeResource = resource;
                operation = nameof(Process);

                KeyValuePair<string, object?>[] pullScopeState =
                [
                    new("Uri", resource.Uri),
                    new("Processor", processor.GetType().Name),
                ];
                using var logScope = _logger.BeginScope(pullScopeState);

                LogPullingResource();
                resource.MarkAsPulling();

                IResourceProcessorResult result =
                    Process(processor, resource, resourcesTracker.Resources);

                ApplyResult(
                    resource,
                    result,
                    unfinishedProcesses,
                    resourcesTracker);
            }

            while (unfinishedProcesses.Count != 0)
            {
                LogBeggingUnfinishProcessBatch();
                List<PendingProcess> unfinishedProcessesNext = [];

                foreach (PendingProcess pendingProcess in unfinishedProcesses)
                {
                    Resource resource = pendingProcess.Resource;
                    IResourceProcessorResult result = pendingProcess.Result;
                    activeResource = resource;
                    operation = nameof(ContinueProcess);
                    KeyValuePair<string, object?>[] continueScopeState =
                    [
                        new("Uri", resource.Uri),
                        new("Processor", result.Processor.GetType().Name),
                    ];
                    using var logScope = _logger.BeginScope(continueScopeState);

                    LogContinueProcess();

                    IResourceProcessorResult nextResult = ContinueProcess(resource, result);
                    ApplyResult(
                        resource,
                        nextResult,
                        unfinishedProcessesNext);
                }

                unfinishedProcesses = unfinishedProcessesNext;
            }

            activeResource = null;
            operation = nameof(ExtractionCompleted);
            Notify(new ExtractionCompleted(resourcesTracker.Resources));
        }
        catch (Exception exception)
        {
            activeResource?.MarkAsFailed();
            NotifyFailureSafely(exception, activeResource, operation);
            throw;
        }
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

    private void ApplyResult(
        Resource resource,
        IResourceProcessorResult result,
        List<PendingProcess> unfinishedProcesses,
        ResourcesTracker? resourcesTracker = null)
    {
        if (resourcesTracker is null && result.GetResourceDependencies().Any())
        {
            throw new InvalidOperationException(
                "Resource dependencies are not allowed in the continue process.");
        }

        if (resourcesTracker is not null)
        {
            foreach (Uri dependency in result.GetResourceDependencies())
            {
                LogAddDependency(dependency);
                resourcesTracker.AddResource(dependency);
            }
        }

        Notify(new ResourceProcessed(resource, result));

        if (result.IsUnfinished)
        {
            LogUnfinishedProcess();
            unfinishedProcesses.Add(new PendingProcess(resource, result));
        }
        else
        {
            LogRessourcePulled();
            resource.MarkAsPulled(result.ExtractionState);
        }
    }

    private sealed class PendingProcess(
        Resource resource,
        IResourceProcessorResult result)
    {
        public Resource Resource { get; } = resource;

        public IResourceProcessorResult Result { get; } = result;
    }

    private void Notify(ExtractionEvent extractionEvent)
    {
        foreach (IExtractionObserver observer in _extractionObservers)
            observer.OnNext(extractionEvent);
    }

    private void NotifyFailureSafely(
        Exception exception,
        Resource? resource,
        string operation)
    {
        ExtractionFailed failure = new(exception, resource, operation);
        foreach (IExtractionObserver observer in _extractionObservers)
        {
            try
            {
                observer.OnNext(failure);
                observer.OnError(exception);
            }
            catch (Exception observerException)
            {
                LogObserverFailure(observerException, observer.GetType().Name);
            }
        }
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
        Level = LogLevel.Error,
        Message = "Extraction observer {Observer} failed while handling an extraction failure.")]
    private partial void LogObserverFailure(Exception exception, string observer);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Adding dependency: {Dependency}")]
    private partial void LogAddDependency(Uri dependency);

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
