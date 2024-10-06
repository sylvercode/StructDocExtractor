namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceDataExtractor<TExtractionData> : IResourceProcessor
{
    IResourceProcessorResult Extract(Resource resource);
}
