
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Default <see cref="IResourceUriTranslater"/> that delegates URI translation to an optional <see cref="IUriTranslater"/>, returning the original URI unchanged when none is provided.</summary>
public class ResourceUriTranslater(IUriTranslater? uriTranslater = null) : IResourceUriTranslater
{
    /// <summary>Gets a no-op <see cref="ResourceUriTranslater"/> that returns every URI unchanged.</summary>
    public static ResourceUriTranslater NoopInstance { get; } = new ResourceUriTranslater();

    /// <summary>Creates a <see cref="ResourceUriTranslater"/> that rewrites URIs by replacing <paramref name="oldBaseUri"/> with <paramref name="newBaseUri"/>.</summary>
    /// <param name="oldBaseUri">The original base URI to replace.</param>
    /// <param name="newBaseUri">The new base URI to use.</param>
    /// <returns>A configured <see cref="ResourceUriTranslater"/> backed by a <see cref="UriBaseTranslater"/>.</returns>
    public static ResourceUriTranslater NewBaseTranslater(
        Uri oldBaseUri,
        Uri newBaseUri) => new(new UriBaseTranslater(oldBaseUri, newBaseUri));

    /// <summary>Gets the inner <see cref="IUriTranslater"/> used by this instance, or <see langword="null"/> for the no-op case.</summary>
    protected IUriTranslater? UriTranslater { get; private set; } = uriTranslater;

    /// <summary>Translates the given URI using the inner <see cref="UriTranslater"/>, or returns it unchanged if no translater is configured.</summary>
    /// <param name="resource">The owning resource (unused in the default implementation).</param>
    /// <param name="uri">The URI to translate.</param>
    /// <returns>The translated URI, or <paramref name="uri"/> if no translater is set.</returns>
    public virtual Uri Translate(Resource resource, Uri uri) =>
        UriTranslater?.Translate(uri) ?? uri;
}
