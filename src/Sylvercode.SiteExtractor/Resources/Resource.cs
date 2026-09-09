using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Represents a site resource (page or asset) identified by its source URI, tracking pull state, URI translation, and attached metadata.</summary>
/// <remarks>
/// A resource is considered pullable when it requires downloading; non-pullable resources (e.g. external links
/// that are only referenced) are tracked but never pulled.  URI translation is delegated to the assigned
/// <see cref="UriTranslater"/>; the no-op translater is used by default.  Fragment URIs can be passed to
/// <see cref="TranslateUri"/> but must share the same base URI as the resource.
/// </remarks>
public class Resource(Uri sourceUri, bool isPullable = false)
{
    /// <summary>Gets the original source URI of this resource.</summary>
    public Uri Uri { get; } = sourceUri;

    /// <summary>Gets the current pull state of this resource.</summary>
    public ResourceState State { get; private set; } = new(isPullable);

    /// <summary>Gets or sets the URI translater used to compute the output path for this resource.</summary>
    public IResourceUriTranslater UriTranslater { get; set; } = ResourceUriTranslater.NoopInstance;

    /// <summary>Gets the translated output URI of this resource using the current <see cref="UriTranslater"/>.</summary>
    public Uri TranslatedResourceUri => TranslateUri(Uri);

    /// <summary>Gets a value indicating whether the translated output URI is identical to the source URI.</summary>
    public bool IsTranslationUriUnchaged => TranslatedResourceUri == Uri;

    /// <summary>Gets the metadata dictionary attached to this resource.</summary>
    public MetadataDictionary Metadata { get; } = [];

    /// <summary>Translates the given URI using this resource's <see cref="UriTranslater"/>.</summary>
    /// <param name="sourceUri">The URI to translate; must be <see langword="null"/> (defaults to <see cref="Uri"/>) or a fragment URI of this resource.</param>
    /// <returns>The translated output URI.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sourceUri"/> is not a fragment of this resource's URI.</exception>
    public Uri TranslateUri(Uri? sourceUri = null)
    {
        Uri uri;
        if (sourceUri is null)
            uri = Uri;
        else
        {
            (Uri uriNoFrag, _) = sourceUri.GetUriAndFragment();
            if (uriNoFrag != Uri)
                throw new ArgumentException("Not a fragment uri of the resouce.", nameof(sourceUri));
            uri = sourceUri;
        }

        return UriTranslater.Translate(this, uri);
    }

    /// <summary>Transitions the resource state to <see cref="ResourcePullState.Pulling"/>.</summary>
    public void MarkAsPulling()
        => State = State.AsPulling();

    /// <summary>Transitions the resource state to <see cref="ResourcePullState.Pulled"/>.</summary>
    public void MarkAsPulled()
        => State = State.AsPulled();
}
