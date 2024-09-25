namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessorResult
{
    IResourceProcessor Processor { get; }
    Resource Resource { get; }

    public void OnPostExtraction();
}
