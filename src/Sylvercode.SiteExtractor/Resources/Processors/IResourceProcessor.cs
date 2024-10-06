namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessor
{
    IResourceProcessorResult Process(Resource resource);
}
