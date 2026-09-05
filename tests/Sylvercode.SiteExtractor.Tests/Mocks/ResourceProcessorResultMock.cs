using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Mock <see cref="IResourceProcessorResult"/> with configurable outcome fields.</summary>
/// <remarks>
/// Supports optional continuation results and call tracking so tests can assert
/// on <see cref="ContinueProcess"/> invocations and final state.
/// </remarks>
public class ResourceProcessorResultMock(
    IResourceProcessor processor,
    Resource resource,
    IEnumerable<Uri>? dependencies = null,
    IResourceProcessorResult? continuProcessResult = null,
    MockCallTracker? callTracker = null) : IResourceProcessorResult
{
    /// <inheritdoc/>
    public IResourceProcessor Processor => processor;

    /// <inheritdoc/>
    public Resource Resource => resource;

    /// <inheritdoc/>
    public MetadataDictionary NewMetadata { get; } = [];

    /// <inheritdoc/>
    public IResourceUriTranslater? ResourceUriTranslaterToSet { get; set; }

    /// <summary>Gets a value indicating whether this result requires a continuation step before it is considered finished.</summary>
    public bool IsUnfinished { get; } = continuProcessResult != null;

    /// <summary>Invokes the next processing step and returns its result, recording the call when a tracker is provided.</summary>
    /// <returns>The configured continuation <see cref="IResourceProcessorResult"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no continuation result has been configured.</exception>
    public IResourceProcessorResult ContinueProcess()
    {
        callTracker?.TrackCall(this, nameof(ContinueProcess), []);

        var result = continuProcessResult ?? throw new InvalidOperationException("No continue process result.");
        continuProcessResult = null;
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<Uri> GetResourceDependencies() => dependencies ?? [];
}
