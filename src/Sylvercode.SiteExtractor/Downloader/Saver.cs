using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Downloader;

public class Saver(ISiteSource<byte[]> siteSource, IDataStore dataStore)
{
    public void Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.SourceUri);
        using var stream = dataStore.GetStream(resource.DestinationUri);
        stream.Write(file);
    }
}
