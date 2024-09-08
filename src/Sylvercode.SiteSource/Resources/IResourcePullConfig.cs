namespace Sylvercode.SiteSource.Resources;

public interface IResourcePullConfig
{
    ResourcePullType GetPullType(Uri uri);
}
