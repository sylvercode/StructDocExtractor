using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourcesTracker(IResourceProcessorProvider processorProvider,
                              IResourceUriRetriver resourceUriRetriver) : IObserver<ExtractionTask>
{
    private readonly ResourceDictionary _resourceDictionary = [];
    public IReadOnlyDictionary<Uri, Resource> Resources => _resourceDictionary;

    private ResourceQueue ResourceQueue { get; } = new();

    public bool AddResource(Uri uri) => AddResource(uri, out _);
    public bool AddResource(Uri uri, out bool hasResourceProcessor)
    {
        IResourceProcessor? resourceProcessor = processorProvider.GetProcessor(uri);
        hasResourceProcessor = resourceProcessor is not null;

        var (resource, isNew) = _resourceDictionary.Add(uri, resourceProcessor is not null);
        if (resourceProcessor is not null && isNew)
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

        AddResource(uri);
    }
    #endregion
}
