using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class BaseExtractorTests_ExtractAll
{
    public class MockExtractor() : BaseExtractor<string, BasicNodeSelectable>(NodeFactoryProviderValue)
    {
        public static readonly SrcNodeFactoryProvider<string, BasicNodeSelectable> NodeFactoryProviderValue = new();

        public delegate ProcessTaskResult<string, BasicNodeSelectable> OnProcessTask(TaskContext taskContext);
        private readonly Queue<OnProcessTask> _onProcessTaskQueue = [];
        public void AddOnProcessTaskAction(OnProcessTask action)
            => _onProcessTaskQueue.Enqueue(action);
        public bool HasOnProcessTask => _onProcessTaskQueue.Count > 0;

        protected override ProcessTaskResult<string, BasicNodeSelectable> ProcessTask(TaskContext taskContext)
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
            result.ProcessResult(taskResult);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(
        ISrcNode node,
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
        IEnumerable<ISrcNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        Assert.Empty(result);
    }

    [Fact]
    public void OneTaskGenaratingARoot_OneRootReturned()
    {
        // Given
        MockExtractor extractor = new();
        extractor.AddTask(DefaultTaskValue);
        extractor.AddOnProcessTaskAction(ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData)));

        // When
        IEnumerable<ISrcNode> result = extractor.ExtractAll();

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
        IEnumerable<ISrcNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        ISrcNode node = Assert.Single(result, r => r.Id == DefaultTaskValue);
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
        IEnumerable<ISrcNode> result = extractor.ExtractAll();

        // Then
        Assert.False(extractor.HasOnProcessTask);
        Assert.Equal(2, result.Count());
        var it = result.GetEnumerator();
        it.MoveNext();
        ISrcNode node = it.Current;
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
}
