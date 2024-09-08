namespace Sylvercode.SiteFetcher.Resources;

public interface IResourcePullConfig
{
    ResourcePullType GetPullType(Uri uri);
}
