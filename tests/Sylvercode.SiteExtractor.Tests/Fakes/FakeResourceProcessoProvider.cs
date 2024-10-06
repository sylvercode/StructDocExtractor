using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;

namespace Sylvercode.SiteExtractor.Tests.Fakes;

public class FakeResourceProcessoProvider(bool returnsProcessor) : IResourceProcessorProvider
{

    public IResourceProcessor? GetProcessor(Uri uri) => returnsProcessor ? new ResourceProcessorMock() : null;
}
