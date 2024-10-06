using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

public class SiteExtractor(
    IResourceProcessorProvider processorProvider) : ISiteExtractor
{
    private readonly ResourcesTracker _resourcesTracker = new(processorProvider);

    public void Extract(Uri uri)
    {
        _resourcesTracker.AddResource(uri, out bool hasResourceProcessor);
        if (!hasResourceProcessor)
            throw new InvalidOperationException("No resource processor found for the given URI.");

        List<IResourceProcessorResult> UnfinishProcess = [];

        while (_resourcesTracker.ResourceQueue.HasQueuedResources)
        {
            var (resource, processor) = _resourcesTracker.ResourceQueue.Dequeue();
            resource.MarkAsPulling();

            IResourceProcessorResult result = processor.Process(resource);

            foreach (var dependency in result.GetResourceDependencies())
                _resourcesTracker.AddResource(dependency);

            if (result.ResourceUriTranslaterToSet != null)
                resource.UriTranslater = result.ResourceUriTranslaterToSet;

            if (result.IsUnfinished)
                UnfinishProcess.Add(result);
            else
                resource.MarkAsPulled();
        }

        while (UnfinishProcess.Count != 0)
        {
            List<IResourceProcessorResult> UnfinishProcessNext = [];

            foreach (var result in UnfinishProcess)
            {
                IResourceProcessorResult nextResult = result.ContinueProcess();

                if (nextResult.GetResourceDependencies().Any())
                    throw new InvalidOperationException("Resource dependencies are not allowed in the continue process.");

                if (nextResult.IsUnfinished)
                    UnfinishProcessNext.Add(nextResult);
                else
                    result.Resource.MarkAsPulled();
            }

            UnfinishProcess = UnfinishProcessNext;
        }
    }
}
