using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

public partial class SiteExtractor(
    IResourceProcessorProvider processorProvider,
    ILogger<SiteExtractor>? logger = null) : ISiteExtractor
{
    private readonly ResourcesTracker _resourcesTracker = new(processorProvider);

    private readonly ILogger<SiteExtractor> _logger = logger ?? NullLogger<SiteExtractor>.Instance;

    public void Extract(Uri uri)
    {
        _resourcesTracker.AddResource(uri, out bool hasResourceProcessor);
        if (!hasResourceProcessor)
            throw new InvalidOperationException("No resource processor found for the given URI.");

        List<IResourceProcessorResult> UnfinishProcess = [];

        while (_resourcesTracker.ResourceQueue.HasQueuedResources)
        {
            var (resource, processor) = _resourcesTracker.ResourceQueue.Dequeue();

            using var logScope = _logger.BeginScope(new List<KeyValuePair<string, object>>
                {
                    new ("Uri", resource.Uri),
                    new ("Processor", processor.GetType().Name),
                });

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
                        LogNewMetadata(name, metadata.GetStrValue());
                }

                resource.Metadata.CopyMetadataFrom(result.NewMetadata);
            }

            if (result.ResourceUriTranslaterToSet != null)
            {
                LogUriTranslaterToSet(result.ResourceUriTranslaterToSet.GetType().Name);
                resource.UriTranslater = result.ResourceUriTranslaterToSet;
            }

            if (result.IsUnfinished)
            {
                LogUnfinishedProcess();
                UnfinishProcess.Add(result);
            }
            else
            {
                LogRessourcePulled();
                resource.MarkAsPulled();
            }
        }

        while (UnfinishProcess.Count != 0)
        {
            LogBeggingUnfinishProcessBatch();
            List<IResourceProcessorResult> UnfinishProcessNext = [];

            foreach (IResourceProcessorResult result in UnfinishProcess)
            {
                using var logScope = _logger.BeginScope(new List<KeyValuePair<string, object>>
                {
                    new ("Uri", result.Resource.Uri),
                    new ("Processor", result.Processor.GetType().Name),
                });

                LogContinueProcess();

                IResourceProcessorResult nextResult = result.ContinueProcess();

                if (nextResult.GetResourceDependencies().Any())
                    throw new InvalidOperationException("Resource dependencies are not allowed in the continue process.");

                if (nextResult.IsUnfinished)
                {
                    LogUnfinishedProcess();
                    UnfinishProcessNext.Add(nextResult);
                }
                else
                {
                    LogRessourcePulled();
                    result.Resource.MarkAsPulled();
                }
            }

            UnfinishProcess = UnfinishProcessNext;
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
