
namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriTranslater"/> that produces a relative URI expressed from a given base URI</summary>
public class UriRelativeFromBaseTranslater(
    Uri baseUri)
    : IUriTranslater
{
    /// <summary>Converts <paramref name="uri"/> to a relative URI expressed from <paramref name="oldBaseUri"/>, preserving query and fragment</summary>
    /// <param name="uri">The absolute URI to make relative; must be rooted under <paramref name="oldBaseUri"/></param>
    /// <param name="oldBaseUri">The base URI from which the result is expressed</param>
    /// <returns>A relative <see cref="Uri"/> including the original query and fragment</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="uri"/> is not rooted under <paramref name="oldBaseUri"/></exception>
    public static Uri Translate(Uri uri, Uri oldBaseUri)
    {
        if (!oldBaseUri.IsBaseOf(uri))
            throw new ArgumentException("Invalid base URI", nameof(uri));

        Uri relative = oldBaseUri.MakeRelativeUri(new Uri(uri.GetLeftPart(UriPartial.Path)));
        return new Uri($"{relative}{uri.Query}{uri.Fragment}", UriKind.Relative);
    }

    /// <inheritdoc/>
    public Uri Translate(Uri uri) => Translate(uri, baseUri);
}
