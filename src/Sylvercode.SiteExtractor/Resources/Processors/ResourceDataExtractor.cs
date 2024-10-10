using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceDataExtractor<TExtractionData>(
    ISiteSource<TExtractionData> siteSource,
    IExtractor<TExtractionData> extractor,
    IDataStore dataStore,
    IStructDocSerializer serisalizer,
    IUriTranslater? uriTranslater = null,
    IReferencerUpdater? referencerUpdater = null) : IResourceDataExtractor<TExtractionData>
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

    private IUriTranslater UriTransler { get; } = uriTranslater ?? new UriBaseTranslater(siteSource.BaseUri, dataStore.BaseUri);

    public IResourceProcessorResult Extract(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources)
    {
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
            return new FinishedProcessResult(this, resource);

        ExtractionTaskObserver observer = new();
        ExtractionResult result = extractor.Extract(extractionData, observer);

        if (result.StructDocNodes.Count == 0)
            return new FinishedProcessResult(this, resource);

        return new DataExtractedProcessorResult<TExtractionData>(this, resource, trackedResources, result, UriTransler, observer.Referencers);
    }

    public IResourceProcessorResult ContinueExtraction(DataExtractedProcessorResult<TExtractionData> lastResult)
    {
        referencerUpdater?.UpdateReferencers(lastResult.Referencers, lastResult.TrackedResources);

        using var stream = dataStore.GetStreamWriter(lastResult.Resource.TranslateUri(dataStore.BaseUri));

        serisalizer.Serialize(stream, lastResult.Result.StructDocNodes[0]); // TODO: Handle multiple nodes

        return new FinishedProcessResult(this, lastResult.Resource);
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource, IReadOnlyDictionary<Uri, Resource> trackedResources) => Extract(resource, trackedResources);
    #endregion
}
