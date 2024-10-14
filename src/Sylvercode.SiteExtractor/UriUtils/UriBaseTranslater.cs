
namespace Sylvercode.SiteExtractor.UriUtils;

public class UriBaseTranslater(
    Uri oldBaseUri,
    Uri newBaseUri)
    : IUriTranslater
{
    public static Uri Translate(Uri uri, Uri oldBaseUri, Uri newBaseUri)
    {
        if (!oldBaseUri.IsBaseOf(uri))
            throw new ArgumentException("Invalid base URI", nameof(uri));

        Uri relative = oldBaseUri.MakeRelativeUri(new Uri(uri.GetLeftPart(UriPartial.Path)));
        UriBuilder uriBuilder = new(new Uri(newBaseUri, relative))
        {
            Query = uri.Query,
            Fragment = uri.Fragment
        };
        return uriBuilder.Uri;
    }

    public Uri Translate(Uri uri) => Translate(uri, oldBaseUri, newBaseUri);
}
