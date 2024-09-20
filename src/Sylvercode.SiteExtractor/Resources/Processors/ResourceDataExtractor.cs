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
        TExtractionData extractionData = siteSource.GetData(resource.SourceUri);
        if (extractionData is null)
            return;

        ExtractionResult result = extractor.Extract(extractionData);
        using var stream = dataStore.GetStreamWriter(resource.DestinationUri);
        serisalizer.Serialize(stream, result.StructDocNodes[0]); // TODO: Handle multiple nodes
    }

    #region IResourceProcessor
    public ResourcePullType GetPullType() => ResourcePullType.Extract;

    public void Process(Resource resource) => Extract(resource);
    #endregion
}
