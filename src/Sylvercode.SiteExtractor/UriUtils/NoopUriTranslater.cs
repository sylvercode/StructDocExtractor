namespace Sylvercode.SiteExtractor.UriUtils;

public class NoopUriTranslater : IUriTranslater
{
    public static NoopUriTranslater Default { get; } = new NoopUriTranslater();
    public Uri Translate(Uri uri, Uri storeBaseUri, Uri? sourceBaseUri = null)
        => uri;
}
