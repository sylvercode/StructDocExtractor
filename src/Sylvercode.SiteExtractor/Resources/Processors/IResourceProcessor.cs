namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessor
{
    ResourcePullType GetPullType();
    void Process(Resource resource);
}
