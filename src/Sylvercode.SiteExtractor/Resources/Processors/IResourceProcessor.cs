namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessor
{
    IResourceProcessorResult Process(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources);
}
