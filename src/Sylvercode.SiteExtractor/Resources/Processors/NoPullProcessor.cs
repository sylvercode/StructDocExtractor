namespace Sylvercode.SiteExtractor.Resources.Processors;

public class NoPullProcessor : IResourceProcessor
{
    public static NoPullProcessor Default { get; } = new NoPullProcessor();

    private NoPullProcessor() { }

    public IResourceProcessorResult Process(Resource resource)
        => new NoPostProcessResult(this, resource);
}
