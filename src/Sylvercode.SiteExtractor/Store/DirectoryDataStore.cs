using Microsoft.Extensions.Options;


namespace Sylvercode.SiteExtractor.Store;

public class DirectoryDataStore(
    IOptions<DirectoryDataStoreOptions> options,
    IOptions<SiteExtractorOptions> siteProcessorOptions) 
    : BaseDataStore(siteProcessorOptions)
{
    private bool MustAutoCreateBaseDir => options.Value.AutoCreateBaseDir;
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
            if (!MustAutoCreateBaseDir)
                throw new DirectoryNotFoundException(path);
            Directory.CreateDirectory(path);
        }
    }
}
