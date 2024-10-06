using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class DataExtractedProcessorResult<TExtractionData>(
    ResourceDataExtractor<TExtractionData> processor,
    Resource resource,
    ExtractionResult result,
    IUriTranslater? resourceUriTranslaterToSet,
    List<IStructDocReferencer> referencers) : BaseResourceProcessorResult(processor, resource, resourceUriTranslaterToSet, isUnfinished: true)
{
    public ExtractionResult Result => result;
    public List<IStructDocReferencer> Referencers { get; } = referencers;
    public override IResourceProcessorResult ContinueProcess() => processor.ContinueExtraction(this);
    public override IEnumerable<Uri> GetResourceDependencies() => Referencers.Select(r => new Uri(r.GetReference()));
}
