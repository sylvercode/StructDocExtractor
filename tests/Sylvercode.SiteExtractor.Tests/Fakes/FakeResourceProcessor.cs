using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests.Fakes;


public class FakeResourceProcessor(ResourcePullType pullType) : IResourceProcessor
{
    public ResourcePullType GetPullType() => pullType;

    public void Process(Resource resource)
    {
    }
}

public class FakeResourceProcessoProvider(ResourcePullType pullType) : IResourceProcessorProvider

{

    public IResourceProcessor GetProcessor(Uri uri) => new FakeResourceProcessor(pullType);
}
