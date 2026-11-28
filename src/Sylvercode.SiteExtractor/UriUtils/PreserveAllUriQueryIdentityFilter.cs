namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Preserves all query parameters in an URI used as an identity.</summary>
public sealed class PreserveAllUriQueryIdentityFilter : IUriQueryIdentityFilter
{
    /// <inheritdoc/>
    public Uri Filter(Uri uri) => uri;
}
