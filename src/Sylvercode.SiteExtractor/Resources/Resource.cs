using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

public class Resource(Uri sourceUri, bool isPullable = false)
{
    public Uri Uri { get; } = sourceUri;

    public ResourceState State { get; private set; } = new(isPullable);

    public Uri? BaseSourceUri { get; set; }

    public IUriTranslater UriTranslater { get; private set; } = NoopUriTranslater.Default;

    public Resource(Uri uri, Uri baseSourceUri, bool isPullable = false)
        : this(uri, isPullable)
    {
        BaseSourceUri = baseSourceUri;
    }

    public Uri TranslateUri(Uri newBaseUri)
        => UriTranslater.Translate(Uri, newBaseUri, BaseSourceUri);

    public void MarkAsPulling()
        => State = State.AsPulling();

    public void MarkAsPulled(IUriTranslater? uriTranslater = null)
    {
        State = State.AsPulled();
        if (uriTranslater is not null)
            UriTranslater = uriTranslater;
    }
}
