namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Defines how query parameters contribute to a canonical URI identity.</summary>
public interface IUriQueryIdentityFilter
{
    /// <summary>Returns an URI whose query has been filtered while preserving other components.</summary>
    /// <param name="uri">The absolute or relative URI to canonicalize.</param>
    /// <returns>The same kind of URI with only identity-relevant query parameters.</returns>
    Uri Filter(Uri uri);
}
