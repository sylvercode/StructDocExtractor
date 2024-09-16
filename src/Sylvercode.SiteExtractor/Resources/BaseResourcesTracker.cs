using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourcesTracker(IResourceProcessorProvider processorProvider,
                              IResourceUriRetriver resourceUriRetriver) : IObserver<ExtractionTask>
{
    private readonly ResourceDictionary _resourceDictionary = [];
    public IReadOnlyDictionary<Uri, Resource> Resources => _resourceDictionary;

    private ResourceQueue ResourceQueue { get; } = new();

    public bool AddResource(Uri uri, IResourceProcessor resourceProcessor)
    {
        var (resource, isNew) = _resourceDictionary.Add(uri, resourceProcessor.GetPullType());
        if (isNew)
            ResourceQueue.Enqueue(resource, resourceProcessor);

        return isNew;
    }

    #region IObserver<ExtractionTask>
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ExtractionTask value)
    {
        ExtractionTaskResult? result = value.TaskResult;
        if (result is null)
            return;

        IStructDocNode? node = result.SrcNode;

        if (node is null)
            return;

        Uri? uri = resourceUriRetriver.GetResourceUri(node);
        if (uri is null)
            return;

        IResourceProcessor resourceProcessor = processorProvider.GetProcessor(uri);
        ResourcePullType pullType = resourceProcessor.GetPullType();
        var (resource, isNew) = _resourceDictionary.Add(uri, pullType);
        if (isNew)
            ResourceQueue.Enqueue(resource, resourceProcessor);
    }
    #endregion
}
