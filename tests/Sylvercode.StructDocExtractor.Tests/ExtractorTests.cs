using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Fakes;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class ExtractorTests
{
    [Fact]
    public void EventFired()
    {
        // Given
        List<string?> CallbackIds = [];
        void OnTaskResultSet(object? sender, EventArgs e) =>
            CallbackIds.Add(((ExtractionTask?)sender)?.TaskResult?.SrcNode?.Id);

        Extractor<FakeStructDocData, BasicNodeDiscriminator> extractor = new(
            BasicStructDocNodeFactoryProvider.Default,
            dataDiscriminatorFactory: FakeStructDocDataDiscriminatorProvider.Default);
        extractor.TaskResultSet += OnTaskResultSet;

        FakeStructDocData data = FakeStructDocData.New().WithId("1")
            .NewChildrenBuilder()
                .WithId("2")
                .NewChildrenBuilder()
                    .WithId("3")
                .BuildChildren()
            .BuildChildren();


        // When
        extractor.Extract(data);

        // Then
        Assert.Collection(CallbackIds,
            id => Assert.Equal("1", id),
            id => Assert.Equal("2", id),
            id => Assert.Equal("3", id));
    }
}
