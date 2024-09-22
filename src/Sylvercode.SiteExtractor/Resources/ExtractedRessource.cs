using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.SiteExtractor.Resources;

public interface IResourceProcessorResult
{
    IResourceProcessor Processor { get; }
    Resource Resource { get; }

    public void OnPostExtraction();
}

public abstract class BaseResourceProcessorResult(IResourceProcessor processor, Resource resource) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;
    public Resource Resource => resource;

    public abstract void OnPostExtraction();
}

public class NoPostProcessResult(IResourceProcessor processor, Resource resource) : BaseResourceProcessorResult(processor, resource)
{
    public override void OnPostExtraction()
    {
        // NOOP
    }
}

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
