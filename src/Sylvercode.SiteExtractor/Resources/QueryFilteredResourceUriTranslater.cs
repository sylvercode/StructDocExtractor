using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Applies query identity filtering before delegating resource URI translation.</summary>
/// <param name="inner">The resource translator receiving the filtered URI.</param>
/// <param name="queryIdentityFilter">The shared source-identity filter.</param>
public sealed class QueryFilteredResourceUriTranslater(
    IResourceUriTranslater inner,
    IUriQueryIdentityFilter queryIdentityFilter) : IResourceUriTranslater
{
    /// <inheritdoc/>
    public Uri Translate(Resource resource, Uri uri)
        => inner.Translate(resource, queryIdentityFilter.Filter(uri));
}
