using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public abstract partial class BaseDataStore(
    IOptions<SiteExtractorOptions> options,
    ILogger? logger = null) : IDataStore
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    public Uri BaseUri => options.Value.GetOutputUri();
    public abstract Stream GetStream(Uri uri);
    public virtual StreamWriter GetStreamWriter(Uri uri) => new(GetStream(uri));

    protected Uri GetCompleteUri(Uri uri)
    {
        if (uri.IsAbsoluteUri
            && BaseUri.IsBaseOf(uri))
        {
            LogUriAlreadyComplete(uri);
            return uri;
        }

        Uri completeUri = new(BaseUri, uri);
        if (!BaseUri.IsBaseOf(completeUri))
            throw new ArgumentException("Uri is not a child of the base uri", nameof(uri));

        LogUriCompleted(uri, completeUri);
        return completeUri;
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {Uri} is already complete")]
    private partial void LogUriAlreadyComplete(Uri uri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {OriginalUri} completd to {Uri}")]
    private partial void LogUriCompleted(Uri originalUri, Uri uri);
}
