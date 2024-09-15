using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Fakes;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class ExtractorTests
{

    class ExtractionObserver : IObserver<ExtractionTask>
    {
        public List<string?> CallbackIds { get; } = [];
        public int OnCompletedCount { get; private set; }

        public void OnNext(ExtractionTask value) =>
            CallbackIds.Add(value.TaskResult?.SrcNode?.Id);

        public void OnError(Exception error) => throw new NotImplementedException();

        public void OnCompleted() => OnCompletedCount++;
    }

    [Fact]
    public void EventFired()
    {
        // Given
        ExtractionObserver observer = new();

        Extractor<FakeStructDocData, BasicNodeDiscriminator> extractor = new(
            BasicStructDocNodeFactoryProvider.Default,
            dataDiscriminatorFactory: FakeStructDocDataDiscriminatorProvider.Default);
        extractor.Subscribe(observer);

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
        Assert.Collection(observer.CallbackIds,
            id => Assert.Equal("1", id),
            id => Assert.Equal("2", id),
            id => Assert.Equal("3", id));

        Assert.Equal(1, observer.OnCompletedCount);
    }
}
