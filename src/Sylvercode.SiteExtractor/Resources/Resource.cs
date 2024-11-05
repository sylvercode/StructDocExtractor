using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources;

public class Resource(Uri sourceUri, bool isPullable = false)
{
    public Uri Uri { get; } = sourceUri;

    public ResourceState State { get; private set; } = new(isPullable);

    public IResourceUriTranslater UriTranslater { get; set; } = ResourceUriTranslater.NoopInstance;

    public Uri TranslatedResourceUri => TranslateUri(Uri);

    public bool IsTranslationUriUnchaged => TranslatedResourceUri == Uri;

    public MetadataDictionary Metadata { get; } = [];

    public Uri TranslateUri(Uri? sourceUri = null)
    {
        Uri uri;
        if (sourceUri is null)
            uri = Uri;
        else
        {
            (uri, _) = sourceUri.GetUriAndFragment();
            if (uri != sourceUri)
                throw new ArgumentException("Not a fragment uri of the resouce.", nameof(sourceUri));
        }

        return UriTranslater.Translate(this, uri);
    }

    public void MarkAsPulling()
        => State = State.AsPulling();

    public void MarkAsPulled()
        => State = State.AsPulled();
}
