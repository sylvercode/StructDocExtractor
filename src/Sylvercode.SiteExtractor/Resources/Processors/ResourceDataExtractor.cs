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
    public void Extract(Resource resource)
    {
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
            return;

        ExtractionResult result = extractor.Extract(extractionData);
        using var stream = dataStore.GetStreamWriter(resource.TranslateUri(dataStore.BaseUri));
        serisalizer.Serialize(stream, result.StructDocNodes[0]); // TODO: Handle multiple nodes
    }

    #region IResourceProcessor
    public void Process(Resource resource) => Extract(resource);
    #endregion
}
