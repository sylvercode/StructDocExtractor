using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class BaseExtractorTests_ExtractAll
{
    public class MockExtractor() : BaseExtractor<string, BasicNodeDiscriminator>(NodeFactoryProviderValue)
    {
        public static readonly StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> NodeFactoryProviderValue = new();

        public delegate ProcessTaskResult<string, BasicNodeDiscriminator> OnProcessTask(TaskContext taskContext);

        private readonly Queue<OnProcessTask> _onProcessTaskQueue = [];

        public void AddOnProcessTaskAction(OnProcessTask action)
            => _onProcessTaskQueue.Enqueue(action);

        public bool HasOnProcessTask => _onProcessTaskQueue.Count > 0;

        protected override IProcessTaskResult<string, BasicNodeDiscriminator> ProcessTask(TaskContext taskContext)
        {
            if (!_onProcessTaskQueue.TryDequeue(out OnProcessTask? nextAction))
                throw new InvalidOperationException("No Next action in queue.");

            return nextAction.Invoke(taskContext);
        }
    }
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);
    public const string DefaultTaskValueA = nameof(DefaultTaskValueA);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.SetResult(taskResult, ChildrenTaskInfoFactory.Default);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(
        IStructDocNode node,
        ICollection<string>? childData = null,
        ICollection<string>? otherData = null)
    {
        BasicProcessTaskResult taskResult = new(node);
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        if (otherData is not null)
            taskResult.ExtraTasksExtractionData.AddRange(otherData);
        return taskResult;
    }

    [Fact]
    public void NoTask_NothingProcessed()
    {
        // Given
        MockExtractor extractor = new();

        // When
        IEnumerable<IStructDocNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        Assert.Empty(result);
    }

    [Fact]
    public void OneTaskGeneratingARoot_OneRootReturned()
    {
        // Given
        MockExtractor extractor = new();
        extractor.AddTask(DefaultTaskValue);
        extractor.AddOnProcessTaskAction(ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData)));

        // When
        IEnumerable<IStructDocNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        Assert.Single(result, r => r.Id == DefaultTaskValue);
    }

    [Fact]
    public void OneTaskWithTwoSubTask_OneRootWithTwoChildrenReturned()
    {
        // Given
        MockExtractor extractor = new();
        extractor.AddTask(DefaultTaskValue);
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2]));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));

        // When
        IEnumerable<IStructDocNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        IStructDocNode node = Assert.Single(result, r => r.Id == DefaultTaskValue);
        BasicSrcRootBlock root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Collection(root.Content,
            c => Assert.Equal(DefaultTaskValue1, Assert.IsType<BasicSrcNode>(c).Id),
            c => Assert.Equal(DefaultTaskValue2, Assert.IsType<BasicSrcNode>(c).Id));
    }

    [Fact]
    public void OneTaskWithTwoSubTaskAndExtraTask_TwoRootReturned()
    {
        // Given
        MockExtractor extractor = new();
        extractor.AddTask(DefaultTaskValue);
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2], [DefaultTaskValueA]));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData)));

        // When
        IEnumerable<IStructDocNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        Assert.Equal(2, result.Count());
        var it = result.GetEnumerator();
        it.MoveNext();
        IStructDocNode node = it.Current;
        Assert.Equal(DefaultTaskValue, node.Id);
        BasicSrcRootBlock root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Collection(root.Content,
            c => Assert.Equal(DefaultTaskValue1, Assert.IsType<BasicSrcNode>(c).Id),
            c => Assert.Equal(DefaultTaskValue2, Assert.IsType<BasicSrcNode>(c).Id));
        it.MoveNext();
        node = it.Current;
        Assert.Equal(DefaultTaskValueA, node.Id);
        root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Empty(root.Content);
    }

    [Fact]
    public void OneTaskWithTwoSubTaskOneWithoutResult_OneRootWithOneChildrenReturned()
    {
        // Given
        MockExtractor extractor = new();
        extractor.AddTask(DefaultTaskValue);
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2]));
        extractor.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        extractor.AddOnProcessTaskAction(
            ctx => ProcessTaskResult.NewSkipped<string, BasicNodeDiscriminator>());

        // When
        IEnumerable<IStructDocNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        IStructDocNode node = Assert.Single(result, r => r.Id == DefaultTaskValue);
        BasicSrcRootBlock root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Single(root.Content, c => Assert.IsType<BasicSrcNode>(c).Id == DefaultTaskValue1);
    }
}


public class BaseExtractorTests_ProcessTask
{
    // TODO: Add tests for ProcessTask
}
