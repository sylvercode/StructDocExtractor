using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Contract for the outcome produced by an <see cref="IResourceProcessor"/> after processing a resource.</summary>
/// <remarks>
/// A result may be unfinished (<see cref="IsUnfinished"/> is <see langword="true"/>), meaning the pipeline must call
/// <see cref="ContinueProcess"/> to complete post-processing steps such as serializing extracted nodes and updating
/// URI references.  <see cref="GetResourceDependencies"/> exposes any child resource URIs discovered during processing.
/// </remarks>
public interface IResourceProcessorResult
{
    /// <summary>Gets the processor that produced this result.</summary>
    IResourceProcessor Processor { get; }

    /// <summary>Gets the resource that was processed.</summary>
    Resource Resource { get; }

    /// <summary>Gets the metadata entries discovered or computed during processing.</summary>
    MetadataDictionary NewMetadata { get; }

    /// <summary>Gets the <see cref="IResourceUriTranslater"/> to assign to the resource after processing, or <see langword="null"/> if no change is needed.</summary>
    IResourceUriTranslater? ResourceUriTranslaterToSet { get; }

    /// <summary>Gets a value indicating whether further post-processing is required via <see cref="ContinueProcess"/>.</summary>
    bool IsUnfinished { get; }

    /// <summary>Performs the next post-processing step and returns the updated result.</summary>
    /// <returns>An <see cref="IResourceProcessorResult"/> reflecting the state after this continuation step.</returns>
    IResourceProcessorResult ContinueProcess();

    /// <summary>Returns the URIs of any dependent resources discovered during processing.</summary>
    /// <returns>An enumerable of dependency URIs to enqueue for further processing, or an empty sequence if none.</returns>
    IEnumerable<Uri> GetResourceDependencies();
}
