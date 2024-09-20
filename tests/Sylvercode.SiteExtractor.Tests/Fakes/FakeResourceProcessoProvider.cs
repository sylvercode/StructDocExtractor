using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Fakes;

public class FakeResourceProcessoProvider(ResourcePullType pullType) : IResourceProcessorProvider

{

    public IResourceProcessor GetProcessor(Uri uri) => new FakeResourceProcessor(pullType);
}
