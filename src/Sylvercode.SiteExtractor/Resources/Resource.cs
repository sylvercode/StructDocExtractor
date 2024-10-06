using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

public class Resource(Uri sourceUri, bool isPullable = false)
{
    public Uri Uri { get; } = sourceUri;

    public ResourceState State { get; private set; } = new(isPullable);

    public IUriTranslater UriTranslater { get; set; } = NoopUriTranslater.Default;

    public Uri TranslateUri(Uri sourceUri)
    {
        var (uri, _) = sourceUri.GetUriAndFragment();
        if (uri != sourceUri)
            throw new ArgumentException("Not a fragment uri of the resouce.", nameof(sourceUri));

        return UriTranslater.Translate(sourceUri);
    }

    public void MarkAsPulling()
        => State = State.AsPulling();

    public void MarkAsPulled()
        => State = State.AsPulled();
}
