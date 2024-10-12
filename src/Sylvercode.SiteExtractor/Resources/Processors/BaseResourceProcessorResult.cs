
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public abstract class BaseResourceProcessorResult(
    IResourceProcessor processor,
    Resource resource,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    bool isUnfinished,
    MetadataDictionary? newMetadatas = null) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;

    public Resource Resource => resource;

    public MetadataDictionary NewMetadata { get; } = newMetadatas ?? [];

    public IResourceUriTranslater? ResourceUriTranslaterToSet => resourceUriTranslaterToSet;

    public bool IsUnfinished => isUnfinished;

    public abstract IResourceProcessorResult ContinueProcess();

    public virtual IEnumerable<Uri> GetResourceDependencies() => [];
}
