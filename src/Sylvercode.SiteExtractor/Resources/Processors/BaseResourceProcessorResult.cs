
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Abstract base for all processor result types, providing shared state for the processor, resource, metadata, URI translater assignment, and completion flag.</summary>
/// <remarks>
/// Concrete subclasses must implement <see cref="ContinueProcess"/>; the base class provides a default empty
/// implementation for <see cref="GetResourceDependencies"/>.  When <see cref="IsUnfinished"/> is
/// <see langword="true"/>, the pipeline will call <see cref="ContinueProcess"/> to finish post-processing.
/// </remarks>
public abstract class BaseResourceProcessorResult(
    IResourceProcessor processor,
    Resource resource,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    bool isUnfinished,
    MetadataDictionary? newMetadatas = null) : IResourceProcessorResult
{
    /// <inheritdoc/>
    public IResourceProcessor Processor => processor;

    /// <inheritdoc/>
    public Resource Resource => resource;

    /// <inheritdoc/>
    public MetadataDictionary NewMetadata { get; } = newMetadatas ?? [];

    /// <inheritdoc/>
    public IResourceUriTranslater? ResourceUriTranslaterToSet => resourceUriTranslaterToSet;

    /// <inheritdoc/>
    public bool IsUnfinished => isUnfinished;

    /// <inheritdoc/>
    public abstract IResourceProcessorResult ContinueProcess();

    /// <inheritdoc/>
    public virtual IEnumerable<Uri> GetResourceDependencies() => [];
}
