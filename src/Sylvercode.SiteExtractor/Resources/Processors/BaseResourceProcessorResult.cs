
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Abstract base for processor results, providing processor, metadata, URI translation, extraction outcome, and completion state.</summary>
/// <remarks>
/// Concrete subclasses must implement <see cref="ContinueProcess"/>; the base class provides a default empty
/// implementation for <see cref="GetResourceDependencies"/>.  When <see cref="IsUnfinished"/> is
/// <see langword="true"/>, the pipeline will call <see cref="ContinueProcess"/> to finish post-processing.
/// </remarks>
public abstract class BaseResourceProcessorResult(
    IResourceProcessor processor,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    bool isUnfinished,
    ResourceExtractionState extractionState,
    MetadataDictionary? newMetadatas = null) : IResourceProcessorResult
{
    /// <inheritdoc/>
    public IResourceProcessor Processor => processor;

    /// <inheritdoc/>
    public MetadataDictionary NewMetadata { get; } = newMetadatas ?? [];

    /// <inheritdoc/>
    public IResourceUriTranslater? ResourceUriTranslaterToSet => resourceUriTranslaterToSet;

    /// <inheritdoc/>
    public ResourceExtractionState ExtractionState => extractionState;

    /// <inheritdoc/>
    public bool IsUnfinished => isUnfinished;

    /// <inheritdoc/>
    public abstract IResourceProcessorResult ContinueProcess(Resource resource);

    /// <inheritdoc/>
    public virtual IEnumerable<Uri> GetResourceDependencies() => [];
}
