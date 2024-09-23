using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Fakes;

public class FakeResourceProcessor() : IResourceProcessor
{
    public IResourceProcessorResult Process(Resource resource)
        => new NoPostProcessResult(this, resource);
}
