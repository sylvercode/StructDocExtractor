using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class ResourceProcessorMock(bool returnsFinishedResultByDefault = true) : IResourceProcessor
{
    private readonly Queue<Func<IResourceProcessor, Resource, IResourceProcessorResult>> _results = new();
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository)
        => (_results.Count > 0)
            ? _results.Dequeue().Invoke(this, resource)
            : returnsFinishedResultByDefault
                ? new ResourceProcessorResultMock(this, resource)
                : throw new InvalidOperationException($"No result for: {resource.Uri}");

    public void AddResult(Func<IResourceProcessor, Resource, IResourceProcessorResult> result)
            => _results.Enqueue(result);

    public bool HasResults => _results.Count > 0;
}
