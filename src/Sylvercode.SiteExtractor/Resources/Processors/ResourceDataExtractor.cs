using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

// TODO: add tests
public class ResourceDataExtractor<TExtractionData>(
    IExtractor<TExtractionData> extractor,
    ISiteSource<TExtractionData> siteSource,
    IStructDocSerializer serisalizer,
    IDataStore dataStore) : IResourceDataExtractor<TExtractionData>
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

    public IResourceProcessorResult Extract(Resource resource)
    {
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
            return new FinishedProcessResult(this, resource);

        ExtractionTaskObserver observer = new();
        ExtractionResult result = extractor.Extract(extractionData, observer);

        return new DataExtractedProcessorResult<TExtractionData>(this, resource, result, new UriBaseTranslater(siteSource.BaseUri, dataStore.BaseUri), observer.Referencers);
    }

    public IResourceProcessorResult OnContinueExtraction(Resource resource, ExtractionResult result)
    {
        using var stream = dataStore.GetStreamWriter(resource.TranslateUri(dataStore.BaseUri));
        serisalizer.Serialize(stream, result.StructDocNodes[0]); // TODO: Handle multiple nodes
        return new FinishedProcessResult(this, resource);
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource) => Extract(resource);
    #endregion
}
