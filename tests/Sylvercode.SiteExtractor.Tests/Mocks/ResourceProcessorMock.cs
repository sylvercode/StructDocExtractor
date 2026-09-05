using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Mock <see cref="IResourceProcessor"/> capturing process calls for verification.</summary>
/// <remarks>
/// Dequeues pre-configured result factories on each <see cref="Process"/> call,
/// or returns a default <see cref="ResourceProcessorResultMock"/> when none are queued.
/// </remarks>
public class ResourceProcessorMock(bool returnsFinishedResultByDefault = true) : IResourceProcessor
{
    private readonly Queue<Func<IResourceProcessor, Resource, IResourceProcessorResult>> _results = new();

    /// <inheritdoc/>
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository)
        => (_results.Count > 0)
            ? _results.Dequeue().Invoke(this, resource)
            : returnsFinishedResultByDefault
                ? new ResourceProcessorResultMock(this, resource)
                : throw new InvalidOperationException($"No result for: {resource.Uri}");

    /// <summary>Enqueues a result factory to be returned on the next <see cref="Process"/> call.</summary>
    /// <param name="result">A factory that receives the processor and resource and returns the configured result.</param>
    public void AddResult(Func<IResourceProcessor, Resource, IResourceProcessorResult> result)
            => _results.Enqueue(result);

    /// <summary>Gets a value indicating whether any pre-configured results remain in the queue.</summary>
    public bool HasResults => _results.Count > 0;
}
