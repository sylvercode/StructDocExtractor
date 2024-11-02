using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class DataExtractedProcessorResult<TExtractionData>(
    ResourceDataExtractor<TExtractionData> processor,
    Resource resource,
    IReadOnlyResourceRepository resourceRepository,
    ExtractionResult result,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    List<IStructDocReferencer> referencers,
    MetadataDictionary newMetadatas)
    : BaseResourceProcessorResult(
        processor,
        resource,
        resourceUriTranslaterToSet,
        isUnfinished: true,
        newMetadatas)
{
    public IReadOnlyResourceRepository ResourceRepository { get; } = resourceRepository;
    public ExtractionResult Result => result;
    public List<IStructDocReferencer> Referencers { get; } = referencers;
    public override IResourceProcessorResult ContinueProcess() 
        => processor.ContinueExtraction(this);
    public override IEnumerable<Uri> GetResourceDependencies() 
        => Referencers.Select(r => new Uri(r.GetReference()));
}
