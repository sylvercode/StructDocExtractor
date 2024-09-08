using Sylvercode.SiteFetcher.Resources;

namespace Sylvercode.SiteFetcher.Tests.Fakes;

public class FakeResourcePullConfig : IResourcePullConfig
{
    public ResourcePullType NestResult { get; set; } = ResourcePullType.NoPull;
    public ResourcePullType GetPullType(Uri uri) => NestResult;
}
