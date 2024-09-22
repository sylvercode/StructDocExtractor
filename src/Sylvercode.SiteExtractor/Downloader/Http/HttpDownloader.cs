using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Downloader.Http;



public class HttpDownloader(HttpClient httpClient) : ISiteSource<byte[]>
{
    public Uri BaseUri { get; } = httpClient.BaseAddress
        ?? throw new ArgumentException("HttpClient must have a BaseAddress", nameof(httpClient));

    public bool CanGetFrom(Uri uri)
        => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    public bool DataExists(Uri uri) => true;

    public bool Download(Uri uri, out byte[] fileBytes)
    {
        try
        {
            fileBytes = httpClient.GetByteArrayAsync(uri).Result;
            return true;
        }
        catch (Exception)
        {
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
}
