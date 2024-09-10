using Sylvercode.SiteSource.Resources;

namespace Sylvercode.SiteSource.Tests.Fakes;

public class FakeResourcePullConfig : IResourcePullConfig
{
    public ResourcePullType NestResult { get; set; } = ResourcePullType.NoPull;
    public ResourcePullType GetPullType(Uri uri) => NestResult;
}
