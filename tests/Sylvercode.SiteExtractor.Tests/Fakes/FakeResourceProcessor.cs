using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Fakes;


public class FakeResourceProcessor(ResourcePullType pullType) : IResourceProcessor
{
    public ResourcePullType GetPullType() => pullType;

    public void Process(Resource resource)
    {
    }
}
