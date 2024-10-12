using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.StructDocExtractor.Metadata;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class ResourceProcessorResultMock(
    IResourceProcessor processor,
    Resource resource,
    IEnumerable<Uri>? dependencies = null,
    IResourceProcessorResult? continuProcessResult = null,
    MockCallTracker? callTracker = null) : IResourceProcessorResult
{
    public IResourceProcessor Processor => processor;

    public Resource Resource => resource;

    public ResourceMetadataDictionary NewMetadata { get; } = [];

    public IResourceUriTranslater? ResourceUriTranslaterToSet { get; set; }

    public bool IsUnfinished { get; } = continuProcessResult != null;

    public IResourceProcessorResult ContinueProcess()
    {
        callTracker?.TrackCall(this, nameof(ContinueProcess), []);

        var result = continuProcessResult ?? throw new InvalidOperationException("No continue process result.");
        continuProcessResult = null;
        return result;
    }

    public IEnumerable<Uri> GetResourceDependencies() => dependencies ?? [];
}
