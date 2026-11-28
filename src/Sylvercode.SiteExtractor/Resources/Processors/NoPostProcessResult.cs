namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Processor result indicating the resource has been fully processed with no further steps required.</summary>
/// <param name="processor">The processor that completed the work.</param>
/// <param name="resourceUriTranslaterToSet">The output translater to apply to the tracked resource.</param>
/// <param name="extractionState">The completed extraction outcome.</param>
/// <param name="outputKind">The kind of output produced.</param>
public class FinishedProcessResult(
    IResourceProcessor processor,
    IResourceUriTranslater? resourceUriTranslaterToSet = null,
    ResourceExtractionState extractionState = ResourceExtractionState.NoOutput,
    ResourceOutputKind outputKind = ResourceOutputKind.None)
    : BaseResourceProcessorResult(
        processor,
        resourceUriTranslaterToSet,
        isUnfinished: false,
        extractionState,
        outputKind)
{
    /// <inheritdoc/>
    public override IResourceProcessorResult ContinueProcess(Resource resource)
        => throw new InvalidOperationException($"Cannot continue processing a {nameof(FinishedProcessResult)}.");
}
