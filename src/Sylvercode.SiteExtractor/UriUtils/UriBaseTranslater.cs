
namespace Sylvercode.SiteExtractor.UriUtils;

// TODO: Add tests.
public class UriBaseTranslater(
    Uri oldBaseUri,
    Uri newBaseUri)
    : IUriTranslater
{
    public static Uri Translate(Uri uri, Uri oldBaseUri, Uri newBaseUri)
    {
        if (!oldBaseUri.IsBaseOf(uri))
            throw new ArgumentException("Invalid base URI", nameof(uri));

        Uri relative = oldBaseUri.MakeRelativeUri(uri);
        return new(newBaseUri, relative);
    }
    
    public Uri Translate(Uri uri) => Translate(uri, oldBaseUri, newBaseUri);
}
