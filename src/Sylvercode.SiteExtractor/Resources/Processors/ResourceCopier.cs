using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public partial class ResourceCopier(
    ISiteSource<byte[]> siteSource,
    IDataStore dataStore,
    IOptions<ResourceCopierOptions> options,
    ILoggerFactory? loggerFactory = null) : IResourceCopiler
{
    public void Download(Uri uri)
    {
        byte[] file = siteSource.GetData(uri);
        using var stream = dataStore.GetStream(UriTranslater.Translate(uri));
        stream.Write(file);
    }

    private CopiedResourceUriTranslater UriTranslater { get; } = new(siteSource, dataStore, options, loggerFactory?.CreateLogger<CopiedResourceUriTranslater>());

    private partial class CopiedResourceUriTranslater(
        ISiteSource<byte[]> siteSource,
        IDataStore dataStore,
        IOptions<ResourceCopierOptions> options,
        ILogger<CopiedResourceUriTranslater>? logger) : IUriTranslater
    {
        private readonly ILogger<CopiedResourceUriTranslater> _logger = logger ?? NullLogger<CopiedResourceUriTranslater>.Instance;

        public Uri Translate(Uri uri)
        {
            _logger.BeginScope((OriginalUri: uri, SourceBaseUri: siteSource.BaseUri, DestinationBaseUri: dataStore.BaseUri));
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

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources)
    {
        Download(resource.Uri);
        return new FinishedProcessResult(this, resource, UriTranslater);
    }
    #endregion
}
