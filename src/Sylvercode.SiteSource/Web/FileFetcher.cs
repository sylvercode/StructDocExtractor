namespace Sylvercode.SiteSource.Web;

public class FileFetcher(HttpClient httpClient)
{
    public bool FetchFile(Uri uri, out byte[] imageBytes)
    {
        try
        {
            imageBytes = httpClient.GetByteArrayAsync(uri).Result;
            return true;
        }
        catch (Exception)
        {
            imageBytes = [];
            return false;
        }
    }
}
