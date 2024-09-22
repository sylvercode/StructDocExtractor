using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopierOptions
{
    public string OutputPath { get; set; } = string.Empty;
    public bool IsOutputPathAbsolute { get; set; } = false;
}

public class ResourceCopier(ISiteSource<byte[]> siteSource, IDataStore dataStore, IOptions<ResourceCopierOptions> options) : IResourceCopiler
{
    public void Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.SourceUri);
        using var stream = dataStore.GetStream(GetDestinationUri(resource.SourceUri));
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
    public ResourcePullType GetPullType() => ResourcePullType.Download;

    public void Process(Resource resource) => Download(resource);
    #endregion
}
