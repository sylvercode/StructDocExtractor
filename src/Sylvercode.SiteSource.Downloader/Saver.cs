using Sylvercode.SiteSource.Resources;
using Sylvercode.SiteSource.Store;

namespace Sylvercode.SiteSource.Downloader;

public class Saver(ISiteSource<byte[]> siteSource, IDataStore dataStore)
{
    public void Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.SourceUri);
        using var stream = dataStore.GetStream(resource.DestinationUri);
        stream.Write(file);
    }
}
