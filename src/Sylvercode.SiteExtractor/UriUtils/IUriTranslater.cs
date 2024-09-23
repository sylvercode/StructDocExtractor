namespace Sylvercode.SiteExtractor.UriUtils;

public interface IUriTranslater
{
    Uri Translate(Uri uri, Uri storeBaseUri, Uri? sourceBaseUri = null);
}
