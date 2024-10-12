
using Sylvercode.StructDocExtractor.Metadata;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public abstract class BaseResourceProcessorResult(
    IResourceProcessor processor,
    Resource resource,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    bool isUnfinished) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;

    public Resource Resource => resource;

    public ResourceMetadataDictionary NewMetadata { get; } = [];

    public IResourceUriTranslater? ResourceUriTranslaterToSet => resourceUriTranslaterToSet;

    public bool IsUnfinished => isUnfinished;

    public abstract IResourceProcessorResult ContinueProcess();

    public virtual IEnumerable<Uri> GetResourceDependencies() => [];
}
