using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopier(ISiteSource<byte[]> siteSource, IDataStore dataStore, IOptions<ResourceCopierOptions> options) : IResourceCopiler
{
    public void Download(Uri uri)
    {
        byte[] file = siteSource.GetData(uri);
        using var stream = dataStore.GetStream(UriTranslater.Translate(uri));
        stream.Write(file);
    }

    private CopiedResourceUriTranslater UriTranslater { get; } = new(siteSource, dataStore, options);

    private class CopiedResourceUriTranslater(ISiteSource<byte[]> siteSource, IDataStore dataStore, IOptions<ResourceCopierOptions> options) : IUriTranslater
    {
        public Uri Translate(Uri uri)
        {
            if (!options.Value.IsOutputPathAbsolute)
                return UriBaseTranslater.Translate(uri, siteSource.BaseUri, dataStore.BaseUri);

            Uri dirUri = new(dataStore.BaseUri, options.Value.OutputPath);

            string path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.ToString();
            string fileName = Path.GetFileName(path);


            return new Uri(dirUri, fileName);
        }
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource)
    {
        Download(resource.Uri);
        return new FinishedProcessResult(this, resource, UriTranslater);
    }
    #endregion
}
