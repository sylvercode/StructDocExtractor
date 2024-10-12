
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceUriTranslater(IUriTranslater? uriTranslater = null) : IResourceUriTranslater
{
    public static ResourceUriTranslater NoopInstance { get; } = new ResourceUriTranslater();
    public static ResourceUriTranslater NewBaseTranslater(
        Uri oldBaseUri,
        Uri newBaseUri) => new(new UriBaseTranslater(oldBaseUri, newBaseUri));

    protected IUriTranslater? UriTranslater { get; private set; } = uriTranslater;

    public Uri Translate(Resource resource, Uri uri) =>
        UriTranslater?.Translate(uri) ?? uri;

}
