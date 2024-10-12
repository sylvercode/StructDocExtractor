using Sylvercode.StructDocExtractor.Metadata;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceProcessorResult
{
    IResourceProcessor Processor { get; }
    Resource Resource { get; }

    ResourceMetadataDictionary NewMetadata { get; }
    IResourceUriTranslater? ResourceUriTranslaterToSet { get; }
    bool IsUnfinished { get; }
    IResourceProcessorResult ContinueProcess();
    IEnumerable<Uri> GetResourceDependencies();
}
