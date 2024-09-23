using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor;

public class SiteExtractor(
    IResourceProcessorProvider processorProvider,
    IResourceUriRetriver resourceUriRetriver) : ISiteExtractor
{
    private readonly ResourcesTracker _resourcesTracker = new(processorProvider, resourceUriRetriver);

    public void Extract(Uri uri)
    {
        _resourcesTracker.AddResource(uri);
        while (_resourcesTracker.ResourceQueue.HasQueuedResources)
        {
            var (resource, processor) = _resourcesTracker.ResourceQueue.Dequeue();
            resource.MarkAsPulling();
            IResourceProcessorResult result = processor.Process(resource);

        }
    }
}
