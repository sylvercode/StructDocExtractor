using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopier(ISiteSource<byte[]> siteSource, IDataStore dataStore) : IResourceCopiler
{
    public IResourceProcessorResult Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.SourceUri);
        using var stream = dataStore.GetStream(resource.DestinationUri);
        stream.Write(file);

        return new NoPostProcessResult(this, resource);
    }

    #region IResourceProcessor
    public ResourcePullType GetPullType() => ResourcePullType.Download;

    public IResourceProcessorResult Process(Resource resource) => Download(resource);
    #endregion
}
