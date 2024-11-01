
namespace Sylvercode.SiteExtractor.UriUtils;

public class UriRelativeFromBaseTranslater(
    Uri baseUri)
    : IUriTranslater
{
    public static Uri Translate(Uri uri, Uri oldBaseUri)
    {
        if (!oldBaseUri.IsBaseOf(uri))
            throw new ArgumentException("Invalid base URI", nameof(uri));

        Uri relative = oldBaseUri.MakeRelativeUri(new Uri(uri.GetLeftPart(UriPartial.Path)));
        return new Uri($"{relative}{uri.Query}{uri.Fragment}", UriKind.Relative);
    }

    public Uri Translate(Uri uri) => Translate(uri, baseUri);
}
