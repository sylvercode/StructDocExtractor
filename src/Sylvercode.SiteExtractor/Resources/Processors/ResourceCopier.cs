using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopier(ISiteSource<byte[]> siteSource, IDataStore dataStore, IOptions<ResourceCopierOptions> options) : IResourceCopiler
{
    public IResourceProcessorResult Download(Resource resource)
    {
        byte[] file = siteSource.GetData(resource.Uri);
        using var stream = dataStore.GetStream(GetDestinationUri(resource.Uri));
        stream.Write(file);

        return new NoPostProcessResult(this, resource);
    }

    private Uri GetDestinationUri(Uri sourceUri)
    {
        if (options.Value.IsOutputPathAbsolute)
            return new Uri(dataStore.BaseUri, options.Value.OutputPath);

        // TODO: refactor this using new translate url
        return new Uri(sourceUri, options.Value.OutputPath);
    }

    private CopiedResourceUriTranslater UriTranslater { get; } = new(this);
    private class CopiedResourceUriTranslater(ResourceCopier copier) : IUriTranslater
    {
        public Uri Translate(Uri uri, Uri storeBaseUri, Uri? sourceBaseUri = null)
        {
            // TODO: implement this
            throw new NotImplementedException();
        }
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource)
    {
        resource.MarkAsPulling();
        Download(resource);
        resource.MarkAsPulled(new CopiedResourceUriTranslater(this));
    }
    #endregion
}
