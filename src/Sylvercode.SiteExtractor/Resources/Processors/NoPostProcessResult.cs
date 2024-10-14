namespace Sylvercode.SiteExtractor.Resources.Processors;

public class FinishedProcessResult(IResourceProcessor processor, Resource resource, IResourceUriTranslater? resourceUriTranslaterToSet = null)
    : BaseResourceProcessorResult(processor, resource, resourceUriTranslaterToSet, isUnfinished: false)
{
    public override IResourceProcessorResult ContinueProcess()
        => throw new InvalidOperationException($"Cannot continue processing a {nameof(FinishedProcessResult)}.");
}
