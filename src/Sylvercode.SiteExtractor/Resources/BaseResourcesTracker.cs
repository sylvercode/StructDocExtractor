using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public abstract class BaseResourcesTracker(ResourceDictionary resourceDictionary,
                                           IResourcePullConfig config) : IObserver<ExtractionTask>
{
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

        Uri? uri = GetUri(node);
        if (uri is null
            || resourceDictionary.ContainsKey(uri))
            return;

        ResourcePullType pullType = config.GetPullType(uri);
        resourceDictionary.Add(uri, pullType);
    }

    protected abstract Uri? GetUri(IStructDocNode node);
}
