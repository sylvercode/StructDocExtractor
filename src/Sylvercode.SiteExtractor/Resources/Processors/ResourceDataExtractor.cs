using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public partial class ResourceDataExtractor<TExtractionData>(
    ISiteSource<TExtractionData> siteSource,
    IExtractor<TExtractionData> extractor,
    IDataStore dataStore,
    IStructDocSerializer serisalizer,
    IResourceUriTranslater? uriTranslater = null,
    IReferencerUpdater? referencerUpdater = null,
    ILogger<ResourceDataExtractor<TExtractionData>>? logger = null) : IResourceDataExtractor<TExtractionData>
{
    private sealed class ExtractionTaskObserver : IObserver<ExtractionTask>
    {
        public List<IStructDocReferencer> Referencers { get; } = [];

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(ExtractionTask value)
        {
            if (value.TaskResult?.SrcNode is IStructDocReferencer referencer)
                Referencers.Add(referencer);
        }
    }

    private readonly ILogger<ResourceDataExtractor<TExtractionData>> _logger = logger ?? new NullLogger<ResourceDataExtractor<TExtractionData>>();

    private IResourceUriTranslater UriTransler { get; } =
        uriTranslater ?? ResourceUriTranslater.NewBaseTranslater(siteSource.BaseUri, dataStore.BaseUri);

    public IResourceProcessorResult Extract(Resource resource, IReadOnlyResourceRepository resourceRepository)
    {
        using var scope = _logger.BeginScope((ResourceUri: resource.Uri, SiteUri: siteSource.BaseUri));
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
        {
            LogNoDataFromSource();
            return new FinishedProcessResult(this, resource);
        }

        ExtractionTaskObserver observer = new();
        ExtractionResult result = extractor.Extract(extractionData, observer);

        if (result.Metadatas.Count != 0)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                foreach (var metadata in result.Metadatas)
                    LogMetadatas(metadata.Key, metadata.Value.GetStrValue());
            }
        }

        if (result.StructDocNodes.Count == 0)
        {
            LogNoNodeFromSource();
            return new FinishedProcessResult(this, resource);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            foreach (IStructDocReferencer referencer in observer.Referencers)
                LogReferencer(referencer.GetReference());
        }

        return new DataExtractedProcessorResult<TExtractionData>(
            this,
            resource,
            resourceRepository,
            result,
            UriTransler,
            observer.Referencers,
            result.Metadatas);
    }

    public IResourceProcessorResult ContinueExtraction(DataExtractedProcessorResult<TExtractionData> lastResult)
    {
        referencerUpdater?.UpdateReferencers(lastResult.Referencers, lastResult.ResourceRepository);

        using var stream = dataStore.GetStreamWriter(lastResult.Resource.TranslateUri(dataStore.BaseUri));

        serisalizer.Serialize(stream, lastResult.Result.StructDocNodes[0]); // TODO: Handle multiple nodes

        return new FinishedProcessResult(this, lastResult.Resource);
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No data found.")]
    private partial void LogNoDataFromSource();

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Trace,
        Message = "Metadata: {Key} = {Value}.")]
    private partial void LogMetadatas(string key, string? value);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No node found.")]
    private partial void LogNoNodeFromSource();

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Debug,
        Message = "Referencer: {Referencer}.")]
    private partial void LogReferencer(string referencer);

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository) => Extract(resource, resourceRepository);
    #endregion
}
