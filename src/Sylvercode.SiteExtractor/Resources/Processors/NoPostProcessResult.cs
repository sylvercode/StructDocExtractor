namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Processor result indicating the resource has been fully processed with no further steps required.</summary>
public class FinishedProcessResult(
    IResourceProcessor processor,
    IResourceUriTranslater? resourceUriTranslaterToSet = null,
    ResourceExtractionState extractionState = ResourceExtractionState.NoOutput)
    : BaseResourceProcessorResult(
        processor,
        resourceUriTranslaterToSet,
        isUnfinished: false,
        extractionState)
{
    /// <inheritdoc/>
    public override IResourceProcessorResult ContinueProcess(Resource resource)
        => throw new InvalidOperationException($"Cannot continue processing a {nameof(FinishedProcessResult)}.");
}
