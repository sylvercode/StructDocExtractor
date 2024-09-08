namespace Sylvercode.SiteFetcher.Resource;

public class StaticResourcePullConfig(ResourcePullType pullType) : IResourcePullConfig
{
    public static StaticResourcePullConfig Default { get; } = new StaticResourcePullConfig(ResourcePullType.NoPull);
    public ResourcePullType GetPullType(Uri uri) => pullType;
}
