namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Removes all query parameters from an URI used as an identity.</summary>
public sealed class RemoveAllUriQueryIdentityFilter : IUriQueryIdentityFilter
{
    /// <inheritdoc/>
    public Uri Filter(Uri uri)
    {
        if (uri.IsAbsoluteUri)
            return new UriBuilder(uri) { Query = string.Empty }.Uri;

        string text = uri.OriginalString;
        int queryIndex = text.IndexOf('?');
        if (queryIndex < 0)
            return uri;

        int fragmentIndex = text.IndexOf('#', queryIndex);
        string filtered = fragmentIndex >= 0
            ? text[..queryIndex] + text[fragmentIndex..]
            : text[..queryIndex];
        return new Uri(filtered, UriKind.Relative);
    }
}
