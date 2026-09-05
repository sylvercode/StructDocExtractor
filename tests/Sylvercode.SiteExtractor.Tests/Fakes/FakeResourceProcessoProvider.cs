using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;

namespace Sylvercode.SiteExtractor.Tests.Fakes;

/// <summary>Fake <see cref="IResourceProcessorProvider"/> returning a fixed processor for all resources.</summary>
public class FakeResourceProcessoProvider(bool returnsProcessor) : IResourceProcessorProvider
{
    /// <summary>Returns a new <see cref="ResourceProcessorMock"/> when configured to provide a processor, or <see langword="null"/> otherwise.</summary>
    /// <param name="uri">The URI of the resource for which a processor is requested.</param>
    /// <returns>A <see cref="ResourceProcessorMock"/> instance, or <see langword="null"/>.</returns>
    public IResourceProcessor? GetProcessor(Uri uri) => returnsProcessor ? new ResourceProcessorMock() : null;
}
