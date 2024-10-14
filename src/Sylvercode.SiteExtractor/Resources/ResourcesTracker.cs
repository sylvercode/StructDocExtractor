using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourcesTracker(IResourceProcessorProvider processorProvider)
{
    private readonly ResourceRepository _resourceRepository = [];
    public IReadOnlyResourceRepository Resources => _resourceRepository;

    public ResourceQueue ResourceQueue { get; } = new();

    public bool AddResource(Uri uri) => AddResource(uri, out _);
    public bool AddResource(Uri uri, out bool hasResourceProcessor)
    {
        IResourceProcessor? resourceProcessor = processorProvider.GetProcessor(uri);
        hasResourceProcessor = resourceProcessor is not null;

        var (resource, isNew) = _resourceRepository.Add(uri, resourceProcessor is not null);
        if (resourceProcessor is not null && isNew)
            ResourceQueue.Enqueue(resource, resourceProcessor);

        return isNew;
    }
}
