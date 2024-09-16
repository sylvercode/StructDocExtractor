namespace Sylvercode.SiteExtractor.Resources.Processors;

public class NoPullProcessor : IResourceProcessor
{
    public static NoPullProcessor Default { get; } = new NoPullProcessor();
    
    private NoPullProcessor() { }
    
    public ResourcePullType GetPullType() => ResourcePullType.NoPull;

    public void Process(Resource resource)
    {
    }
}
