using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class DataExtractedProcessorResult<TExtractionData>(
    IResourceDataExtractor<TExtractionData> processor,
    Resource resource,
    ExtractionResult result) : BaseResourceProcessorResult(processor, resource)
{
    public new IResourceDataExtractor<TExtractionData> Processor => (IResourceDataExtractor<TExtractionData>)base.Processor;
    public ExtractionResult Result => result;
    public override void OnPostExtraction()
    {
        Processor.OnPostExtraction(Resource, Result);
    }
}
