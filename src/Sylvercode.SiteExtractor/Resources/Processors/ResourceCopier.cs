using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public partial class ResourceCopier : IResourceCopiler
{
    private readonly ISiteSource<byte[]> _siteSource;

    private readonly IDataStore _dataStore;

    private readonly IUriTranslater _uriTranslater;

    private readonly ResourceUriTranslater _resourceUriTranslater;

    public ResourceCopier(
        ISiteSource<byte[]> siteSource,
        IDataStore dataStore,
        IOptions<ResourceCopierOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _siteSource = siteSource;
        _dataStore = dataStore;
        _uriTranslater = NewUriTranslater(options.Value, siteSource.BaseUri, loggerFactory);
        _resourceUriTranslater = new(_uriTranslater);
    }

    public void Download(Uri uri)
    {
        byte[] file = _siteSource.GetData(uri);
        using var stream = _dataStore.GetStream(_uriTranslater.Translate(uri));
        stream.Write(file);
    }

    private static IUriTranslater NewUriTranslater(
        ResourceCopierOptions options,
        Uri SourceBaseUri,
        ILoggerFactory? loggerFactory)
    {
        if (options.IsOutputPathAbsolute)
        {
            return new ToRootUriTranslater(options.OutputPath,
                loggerFactory?.CreateLogger<ToRootUriTranslater>());
        }
        else
        {
            return new RelativeUriTranslater(SourceBaseUri,
                options.OutputPath,
                loggerFactory?.CreateLogger<RelativeUriTranslater>());
        }
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository)
    {
        Download(resource.Uri);
        return new FinishedProcessResult(this, resource, _resourceUriTranslater);
    }
    #endregion
}
