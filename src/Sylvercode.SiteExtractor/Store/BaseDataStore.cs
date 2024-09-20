using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public abstract class BaseDataStore(IOptions<SiteExtractorOptions> options) : IDataStore
{
    public Uri BaseUri => new(options.Value.OutputDirectory);
    public abstract Stream GetStream(Uri uri);
    public virtual StreamWriter GetStreamWriter(Uri uri) => new(GetStream(uri));

    protected Uri GetCompleteUri(Uri uri)
    {
        if (BaseUri.IsBaseOf(uri))
            return uri;

        Uri completeUri = new(BaseUri, uri);
        if (!BaseUri.IsBaseOf(completeUri))
            throw new ArgumentException("Uri is not a child of the base uri", nameof(uri));

        return completeUri;
    }
}
