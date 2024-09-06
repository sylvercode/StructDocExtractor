using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Fakes;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class ExtractorTaskSequencerHandlerTests_OnProcessTask
{
    public const string Child1 = nameof(Child1);
    public const string Child2 = nameof(Child2);
    public const string DataChild1 = nameof(DataChild1);
    public const string DataChild2 = nameof(DataChild2);

    public class MockTaskContext(ExtractionTask task)
        : TaskContext<FakeStructDocData, BasicNodeDiscriminator>(task, BasicStructDocNodeFactoryProvider.Default)
    {

    }

    private static ExtractorTaskSequencerHandler<FakeStructDocData, BasicNodeDiscriminator> NewHandler()
    {
        return new(new StructDocNodeFactoryProvider<FakeStructDocData, BasicNodeDiscriminator>(),
                    dataDiscriminatorFactory: FakeStructDocDataDiscriminatorProvider.Default);
    }

    [Fact]
    public void EmptyRootData_ReturnsEmptyRootNode()
    {
        // Given
        FakeStructDocData rootData = FakeStructDocData.New();
        ExtractorTaskSequencerHandler<FakeStructDocData, BasicNodeDiscriminator> handler = NewHandler();

        // When
        IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result =
            handler.OnProcessTask(new MockTaskContext(rootData.ToTask()));

        // Then
        Assert.Equal(TaskResultType.Success, result.ResultType);
        var node = Assert.IsType<BasicSrcRootBlock>(result.SrcNode);
        Assert.Empty(node.Id);
        Assert.Empty(node.Content);
        Assert.Empty(result.SubTasksExtractionData);
        Assert.Empty(result.ExtraTasksExtractionData);
    }

    [Fact]
    public void DataChild_ReturnsDataNode()
    {
        // Given
        FakeStructDocData.New()
            .WithChild(Child1, DataChild1, out FakeStructDocData childData)
            .Build();
        ExtractorTaskSequencerHandler<FakeStructDocData, BasicNodeDiscriminator> handler = NewHandler();

        // When
        IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result =
            handler.OnProcessTask(new MockTaskContext(childData.ToTask()));

        // Then
        Assert.Equal(TaskResultType.Success, result.ResultType);
        var node = Assert.IsType<BasicSrcNode>(result.SrcNode);
        Assert.Equal(Child1, node.Id);
        Assert.Equal(DataChild1, node.Data);
        Assert.Empty(result.SubTasksExtractionData);
        Assert.Empty(result.ExtraTasksExtractionData);
    }

    [Fact]
    public void RootWithTwoChild_ReturnTwoSubtask()
    {
        // Given
        FakeStructDocData rootData = FakeStructDocData.New()
            .WithChild(Child1, DataChild1)
            .WithChild(Child2, DataChild2)
            .Build();
        ExtractorTaskSequencerHandler<FakeStructDocData, BasicNodeDiscriminator> handler = NewHandler();

        // When
        IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result =
            handler.OnProcessTask(new MockTaskContext(rootData.ToTask()));

        // Then
        Assert.Equal(TaskResultType.Success, result.ResultType);
        var root = Assert.IsType<BasicSrcRootBlock>(result.SrcNode);
        Assert.Collection(result.SubTasksExtractionData,
            data => Assert.Equal(Child1, data.Id),
            data => Assert.Equal(Child2, data.Id));
        Assert.Empty(result.ExtraTasksExtractionData);
    }
}
