
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public abstract class BaseResourceProcessorResult(IResourceProcessor processor, Resource resource, IUriTranslater? resourceUriTranslaterToSet, bool isUnfinished) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;

    public Resource Resource => resource;

    public IUriTranslater? ResourceUriTranslaterToSet => resourceUriTranslaterToSet;

    public bool IsUnfinished => isUnfinished;

    public abstract IResourceProcessorResult ContinueProcess();

    public virtual IEnumerable<Uri> GetResourceDependencies() => [];
}
