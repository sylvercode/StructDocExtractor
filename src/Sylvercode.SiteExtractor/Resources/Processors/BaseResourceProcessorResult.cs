namespace Sylvercode.SiteExtractor.Resources.Processors;

public abstract class BaseResourceProcessorResult(IResourceProcessor processor, Resource resource) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;
    public Resource Resource => resource;

    public abstract void OnPostExtraction();
}
