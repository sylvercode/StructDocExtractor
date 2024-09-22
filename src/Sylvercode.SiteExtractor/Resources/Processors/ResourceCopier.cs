using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopier(ISiteSource<byte[]> siteSource, IDataStore dataStore, IOptions<ResourceCopierOptions> options) : IResourceCopiler
{
    public void Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.Uri);
        using var stream = dataStore.GetStream(GetDestinationUri(resource.Uri));
        stream.Write(file);
    }

    private Uri GetDestinationUri(Uri sourceUri)
    {
        if (options.Value.IsOutputPathAbsolute)
            return new Uri(dataStore.BaseUri, options.Value.OutputPath);

        // TODO: refactor this using new translate url
        return new Uri(sourceUri, options.Value.OutputPath);
    }

    #region IResourceProcessor
    public void Process(Resource resource) => Download(resource);
    #endregion
}
