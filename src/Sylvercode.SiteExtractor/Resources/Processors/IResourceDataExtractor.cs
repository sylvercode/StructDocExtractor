namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceDataExtractor<TExtractionData> : IResourceProcessor
{
    void Extract(Resource resource);
}
