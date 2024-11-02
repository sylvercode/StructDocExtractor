using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public partial class ResourceCopier : IResourceCopiler
{
    private sealed partial class CopiedResourceUriTranslater(
        ISiteSource<byte[]> siteSource,
        IDataStore dataStore,
        IOptions<ResourceCopierOptions> options,
        ILogger<CopiedResourceUriTranslater>? logger) : IUriTranslater
    {
        private readonly ILogger<CopiedResourceUriTranslater> _logger = logger ?? NullLogger<CopiedResourceUriTranslater>.Instance;

        public Uri Translate(Uri uri)
        {
            using var scope = _logger.BeginScope((OriginalUri: uri, SourceBaseUri: siteSource.BaseUri, DestinationBaseUri: dataStore.BaseUri));
            if (!options.Value.IsOutputPathAbsolute)
            {
                Uri absoluteResult = UriBaseTranslater.Translate(uri, siteSource.BaseUri, dataStore.BaseUri);
                LogAbsoluteTranslation(absoluteResult);
                return absoluteResult;
            }

            Uri dirUri = new(dataStore.BaseUri, options.Value.OutputPath);
            LogRelativeDirDestination(dirUri);

            string path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.ToString();
            string fileName = Path.GetFileName(path);
            LogFileNameDestiantion(fileName);

            Uri relativeResult = new(dirUri, fileName);
            LogRelativeTransation(relativeResult);
            return relativeResult;
        }

        [LoggerMessage(
            LogLevel.Debug,
            Message = "Absolute translation result: {Result}")]
        private partial void LogAbsoluteTranslation(Uri result);

        [LoggerMessage(
            LogLevel.Debug,
            Message = "Relative translation result: {Result}")]
        private partial void LogRelativeTransation(Uri result);

        [LoggerMessage(
            LogLevel.Trace,
            Message = "Relative directory destination: {DirUri}")]
        private partial void LogRelativeDirDestination(Uri dirUri);

        [LoggerMessage(
            LogLevel.Trace,
            Message = "File name destination: {FileName}")]
        private partial void LogFileNameDestiantion(string fileName);
    }

    private readonly ISiteSource<byte[]> _siteSource;

    private readonly IDataStore _dataStore;

    private readonly CopiedResourceUriTranslater _uriTranslater;

    private readonly ResourceUriTranslater _resourceUriTranslater;

    public ResourceCopier(
        ISiteSource<byte[]> siteSource,
        IDataStore dataStore,
        IOptions<ResourceCopierOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _siteSource = siteSource;
        _dataStore = dataStore;
        _uriTranslater = new(siteSource, dataStore, options, loggerFactory?.CreateLogger<CopiedResourceUriTranslater>());
        _resourceUriTranslater = new(_uriTranslater);
    }

    public void Download(Uri uri)
    {
        byte[] file = _siteSource.GetData(uri);
        using var stream = _dataStore.GetStream(_uriTranslater.Translate(uri));
        stream.Write(file);
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository)
    {
        Download(resource.Uri);
        return new FinishedProcessResult(this, resource, _resourceUriTranslater);
    }
    #endregion
}
