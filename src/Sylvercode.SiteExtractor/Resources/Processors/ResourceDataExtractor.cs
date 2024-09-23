using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceDataExtractor<TExtractionData>(
    IExtractor<TExtractionData> extractor,
    ISiteSource<TExtractionData> siteSource,
    IStructDocSerializer serisalizer,
    IDataStore dataStore) : IResourceDataExtractor<TExtractionData>
{
    public IResourceProcessorResult Extract(Resource resource)
    {
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
            return new NoPostProcessResult(this, resource);

        ExtractionResult result = extractor.Extract(extractionData);

        return new DataExtractedProcessorResult<TExtractionData>(this, resource, result);
    }

    public void OnPostExtraction(Resource resource, ExtractionResult result)
    {
        using var stream = dataStore.GetStreamWriter(resource.TranslateUri(dataStore.BaseUri));
        serisalizer.Serialize(stream, result.StructDocNodes[0]); // TODO: Handle multiple nodes
    }

    #region IResourceProcessor
    public IResourceProcessorResult Process(Resource resource) => Extract(resource);
    #endregion
}
