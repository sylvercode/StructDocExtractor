namespace Sylvercode.SiteSource.Store;

public class DirectoryDataStore(Uri baseUri, bool autoCreateBaseDir = false) : BaseDataStop(baseUri)
{

    public override Stream GetStream(Uri uri)
    {
        Uri completeUri = GetCompleteUri(uri);
        EnsureDirectoryExists(completeUri);
        return File.OpenRead(Path.GetFullPath(completeUri.LocalPath));
    }

    private void EnsureDirectoryExists(Uri uri)
    {
        string path = uri.LocalPath;
        if (!Directory.Exists(path))
        {
            if (!autoCreateBaseDir)
                throw new DirectoryNotFoundException(path);
            Directory.CreateDirectory(path);
        }
    }
}
