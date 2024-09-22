namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessor
{
    ResourcePullType GetPullType();
    IResourceProcessorResult Process(Resource resource);
}
