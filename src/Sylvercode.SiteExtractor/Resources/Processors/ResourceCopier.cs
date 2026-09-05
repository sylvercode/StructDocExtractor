using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Implementation of <see cref="IResourceCopiler"/> that downloads raw resource bytes from a site source and writes them verbatim to the data store.</summary>
/// <remarks>
/// The output path for each resource is resolved from <see cref="ResourceCopierOptions"/>: when
/// <see cref="ResourceCopierOptions.IsOutputPathAbsolute"/> is <see langword="true"/> a
/// <see cref="StaticDirUriTranslater"/> is used; otherwise a <see cref="RelativeUriTranslater"/> resolves
/// the path relative to the source base URI.  The resolved <see cref="ResourceUriTranslater"/> is applied
/// to the resource so that subsequent URI-translation calls reflect the copied output location.
/// </remarks>
public partial class ResourceCopier : IResourceCopiler
{
    private readonly ISiteSource<byte[]> _siteSource;

    private readonly IDataStore _dataStore;

    private readonly IUriTranslater _uriTranslater;

    private readonly ResourceUriTranslater _resourceUriTranslater;

    /// <summary>Initializes a new instance of <see cref="ResourceCopier"/>.</summary>
    /// <param name="siteSource">The binary site source used to fetch raw resource bytes.</param>
    /// <param name="dataStore">The data store where copied bytes are written.</param>
    /// <param name="options">Options controlling the output path and whether it is absolute or relative.</param>
    /// <param name="loggerFactory">Optional factory used to create loggers for URI translater components.</param>
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

    /// <summary>Downloads the resource at <paramref name="uri"/> from the site source and writes its bytes to the data store.</summary>
    /// <param name="uri">The URI of the resource to copy.</param>
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
            return new StaticDirUriTranslater(options.OutputPath,
                loggerFactory?.CreateLogger<StaticDirUriTranslater>());
        }
        else
        {
            return new RelativeUriTranslater(SourceBaseUri,
                options.OutputPath,
                loggerFactory?.CreateLogger<RelativeUriTranslater>());
        }
    }

    #region IResourceProcessor
    /// <inheritdoc/>
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository)
    {
        Download(resource.Uri);
        return new FinishedProcessResult(this, resource, _resourceUriTranslater);
    }
    #endregion
}
