using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Downloader.Http;

/// <summary>
/// <see cref="ISiteSource{T}"/> implementation that downloads binary resource content
/// from HTTP and HTTPS URIs using a configured <see cref="HttpClient"/>.
/// </summary>
/// <remarks>
/// Serves as the HTTP transport layer in the site-source pipeline. Relies on an injected
/// <see cref="HttpClient"/> that must have <see cref="HttpClient.BaseAddress"/> set;
/// the address is typically provided by <see cref="SiteExtractorOptions"/>.
/// Download failures are caught and logged rather than propagated; callers should use
/// <see cref="Download"/> to distinguish success from failure.
/// </remarks>
public partial class HttpDownloader(HttpClient httpClient, ILogger<HttpDownloader>? logger) : ISiteSource<byte[]>
{
    private readonly ILogger<HttpDownloader> _logger = logger ?? NullLogger<HttpDownloader>.Instance;

    /// <summary>Gets the base address of the underlying <see cref="HttpClient"/></summary>
    public Uri BaseUri { get; } = httpClient.BaseAddress
        ?? throw new ArgumentException("HttpClient must have a BaseAddress", nameof(httpClient));

    /// <summary>Returns <see langword="true"/> when <paramref name="uri"/> uses the <c>http</c> or <c>https</c> scheme</summary>
    public bool CanGetFrom(Uri uri)
        => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    /// <summary>Always returns <see langword="true"/>; existence is validated optimistically on the first download attempt</summary>
    public bool DataExists(Uri uri) => true;

    /// <summary>
    /// Downloads the resource at <paramref name="uri"/> as a byte array and assigns it to
    /// <paramref name="fileBytes"/>, returning <see langword="true"/> on success or
    /// <see langword="false"/> and an empty array on failure.
    /// </summary>
    /// <param name="uri">The absolute URI of the resource to download.</param>
    /// <param name="fileBytes">
    /// When this method returns <see langword="true"/>, contains the downloaded bytes;
    /// otherwise an empty array.
    /// </param>
    /// <returns><see langword="true"/> if the download succeeded; otherwise <see langword="false"/>.</returns>
    public bool Download(Uri uri, out byte[] fileBytes)
    {
        using var scope = _logger.BeginScope(new { Uri = uri });
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

    /// <summary>
    /// Downloads and returns the raw bytes of the resource at <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The absolute URI of the resource to fetch.</param>
    /// <returns>The downloaded byte content of the resource.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the download fails.</exception>
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
