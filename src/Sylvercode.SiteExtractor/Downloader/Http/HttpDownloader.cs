using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Downloader.Http;

public partial class HttpDownloader(HttpClient httpClient, ILogger<HttpDownloader>? logger) : ISiteSource<byte[]>
{
    private readonly ILogger<HttpDownloader> _logger = logger ?? NullLogger<HttpDownloader>.Instance;

    public Uri BaseUri { get; } = httpClient.BaseAddress
        ?? throw new ArgumentException("HttpClient must have a BaseAddress", nameof(httpClient));

    public bool CanGetFrom(Uri uri)
        => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    public bool DataExists(Uri uri) => true;

    public bool Download(Uri uri, out byte[] fileBytes)
    {
        _logger.BeginScope(new { Uri = uri });
        try
        {
            fileBytes = httpClient.GetByteArrayAsync(uri).Result;
            LogGetSuccess(uri);
            return true;
        }
        catch (Exception e)
        {
            LogGetFailure(uri, e);
            fileBytes = [];
            return false;
        }
    }

    public byte[] GetData(Uri uri)
    {
        if (!Download(uri, out byte[] fileBytes))
            throw new InvalidOperationException("Failed to fetch file at " + uri);

        return fileBytes;
    }

    [LoggerMessage(
        LogLevel.Trace,
        Message = "Successfully fetched file at {Uri}")]
    private partial void LogGetSuccess(Uri uri);

    [LoggerMessage(
        LogLevel.Debug,
        Message = "Failed to fetch file at {Uri}")]
    private partial void LogGetFailure(Uri uri, Exception exception);
}
