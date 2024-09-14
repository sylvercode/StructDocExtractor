namespace Sylvercode.SiteExtractor.Resources;

public interface IResourcePullConfig
{
    ResourcePullType GetPullType(Uri uri);
}
