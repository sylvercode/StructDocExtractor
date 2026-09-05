namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Processor result indicating the resource has been fully processed with no further steps required.</summary>
public class FinishedProcessResult(IResourceProcessor processor, Resource resource, IResourceUriTranslater? resourceUriTranslaterToSet = null)
    : BaseResourceProcessorResult(processor, resource, resourceUriTranslaterToSet, isUnfinished: false)
{
    /// <inheritdoc/>
    public override IResourceProcessorResult ContinueProcess()
        => throw new InvalidOperationException($"Cannot continue processing a {nameof(FinishedProcessResult)}.");
}
