using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public partial class ResourceDataExtractor<TExtractionData>(
    ISiteSource<TExtractionData> siteSource,
    IExtractor<TExtractionData> extractor,
    IDataStore dataStore,
    IStructDocSerializer serisalizer,
    IUriTranslater? uriTranslater = null,
    IReferencerUpdater? referencerUpdater = null,
    ILogger<ResourceDataExtractor<TExtractionData>>? logger = null) : IResourceDataExtractor<TExtractionData>
{
    private class ExtractionTaskObserver : IObserver<ExtractionTask>
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

    private IUriTranslater UriTransler { get; } = uriTranslater ?? new UriBaseTranslater(siteSource.BaseUri, dataStore.BaseUri);

    public IResourceProcessorResult Extract(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources)
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

        return new DataExtractedProcessorResult<TExtractionData>(this, resource, trackedResources, result, UriTransler, observer.Referencers);
    }

    public IResourceProcessorResult ContinueExtraction(DataExtractedProcessorResult<TExtractionData> lastResult)
    {
        referencerUpdater?.UpdateReferencers(lastResult.Referencers, lastResult.TrackedResources);

        using var stream = dataStore.GetStreamWriter(lastResult.Resource.TranslateUri(dataStore.BaseUri));

        serisalizer.Serialize(stream, lastResult.Result.StructDocNodes[0]); // TODO: Handle multiple nodes

        return new FinishedProcessResult(this, lastResult.Resource);
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No data found.")]
    private partial void LogNoDataFromSource();

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
    public IResourceProcessorResult Process(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources) => Extract(resource, trackedResources);
    #endregion
}
