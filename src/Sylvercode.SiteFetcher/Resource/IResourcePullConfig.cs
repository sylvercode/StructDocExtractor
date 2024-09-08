namespace Sylvercode.SiteFetcher.Resource;

public interface IResourcePullConfig
{
    ResourcePullType GetPullType(Uri uri);
}
