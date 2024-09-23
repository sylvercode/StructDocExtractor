using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Tests.Fakes;

public class FakeResourceProcessoProvider(bool returnsProcessor) : IResourceProcessorProvider
{

    public IResourceProcessor? GetProcessor(Uri uri) => returnsProcessor ? new FakeResourceProcessor() : null;
}
