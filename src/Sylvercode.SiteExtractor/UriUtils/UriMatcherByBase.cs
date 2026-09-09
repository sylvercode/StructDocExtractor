namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriMatcher"/> that accepts URIs whose base matches a configured root URI</summary>
public class UriMatcherByBase(Uri baseUri) : IUriMatcher
{
    /// <inheritdoc/>
    public bool IsMatching(Uri uri) => baseUri.IsBaseOf(uri);
}
