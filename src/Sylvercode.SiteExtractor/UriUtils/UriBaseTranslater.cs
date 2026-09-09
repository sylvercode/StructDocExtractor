
namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriTranslater"/> that rewrites a URI by swapping its base from one root to another</summary>
public class UriBaseTranslater(
    Uri oldBaseUri,
    Uri newBaseUri)
    : IUriTranslater
{
    /// <summary>Translates <paramref name="uri"/> by replacing <paramref name="oldBaseUri"/> with <paramref name="newBaseUri"/>, preserving query and fragment</summary>
    /// <param name="uri">The URI to translate; must be rooted under <paramref name="oldBaseUri"/></param>
    /// <param name="oldBaseUri">The current base URI to remove from <paramref name="uri"/></param>
    /// <param name="newBaseUri">The replacement base URI to prepend</param>
    /// <returns>The translated URI with the new base, original query, and original fragment</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="uri"/> is not rooted under <paramref name="oldBaseUri"/></exception>
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

    /// <inheritdoc/>
    public Uri Translate(Uri uri) => Translate(uri, oldBaseUri, newBaseUri);
}
