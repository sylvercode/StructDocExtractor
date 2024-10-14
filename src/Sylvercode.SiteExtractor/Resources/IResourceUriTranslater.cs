namespace Sylvercode.SiteExtractor.Resources;

public interface IResourceUriTranslater
{
    Uri Translate(Resource resource, Uri uri);
}
