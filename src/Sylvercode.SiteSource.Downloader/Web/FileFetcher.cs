namespace Sylvercode.SiteSource.Downloader.Web;

public class FileFetcher(HttpClient httpClient) : ISiteSource<byte[]>
{
    public bool CanGetFrom(Uri uri)
        => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    public bool DataExists(Uri uri) => true;

    public bool FetchFile(Uri uri, out byte[] fileBytes)
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
        if (!FetchFile(uri, out byte[] fileBytes))
            throw new InvalidOperationException("Failed to fetch file at " + uri);

        return fileBytes;
    }
}
