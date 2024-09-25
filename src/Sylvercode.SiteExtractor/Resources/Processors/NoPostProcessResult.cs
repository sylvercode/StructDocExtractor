namespace Sylvercode.SiteExtractor.Resources.Processors;

public class NoPostProcessResult(IResourceProcessor processor, Resource resource) : BaseResourceProcessorResult(processor, resource)
{
    public override void OnPostExtraction()
    {
        // NOOP
    }
}
