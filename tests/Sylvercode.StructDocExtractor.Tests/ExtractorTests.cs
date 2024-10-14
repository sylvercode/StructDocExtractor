using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Metadatas;
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

        FakeStructDocData data = FakeStructDocData.New().WithId("1")
            .WithMetadata("RootKey", "RootValue")
            .NewChildrenBuilder()
                .WithId("2")
                .WithMetadata("MetaKey", "MetaValue")
                .NewChildrenBuilder()
                    .WithId("3")
                    .WithMetadata("MetaKey", "OverrideValue")
                .BuildChildren()
            .BuildChildren();


        // When
        ExtractionResult result = extractor.Extract(data, observer);

        // Then
        var rootNode = Assert.IsType<BasicSrcRootBlock>(Assert.Single(result.StructDocNodes));
        Assert.Equal("1", rootNode.Id);
        Assert.Collection(rootNode.Content,
            node =>
            {
                var block = Assert.IsType<BasicSrcBloc>(node);
                Assert.Equal("2", block.Id);
                var child = Assert.Single(block.Content);
                Assert.Equal("3", child.Id);
            });

        Assert.Equal(2, result.Metadatas.Count);
        Metadata rootKey = Assert.Contains("RootKey", result.Metadatas);
        Assert.Equal("RootValue", rootKey.GetStrValue());
        Metadata metaKey = Assert.Contains("MetaKey", result.Metadatas);
        Assert.Equal("OverrideValue", metaKey.GetStrValue());

        Assert.Collection(observer.CallbackIds,
            id => Assert.Equal("1", id),
            id => Assert.Equal("2", id),
            id => Assert.Equal("3", id));

        Assert.Equal(1, observer.OnCompletedCount);
    }
}
