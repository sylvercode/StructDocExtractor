using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceDataExtractor<TExtractionData> : IResourceProcessor
{
    IResourceProcessorResult Extract(Resource resource);
    void OnPostExtraction(Resource resource, ExtractionResult result);
}
