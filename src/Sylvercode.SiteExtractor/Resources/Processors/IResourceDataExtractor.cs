using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceDataExtractor<TExtractionData> : IResourceProcessor
{
    IResourceProcessorResult Extract(Resource resource);
    IResourceProcessorResult OnContinueExtraction(Resource resource, ExtractionResult result);
}
