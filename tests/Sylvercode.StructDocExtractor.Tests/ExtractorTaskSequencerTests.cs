using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class ExtractorTaskSequencerTests_ExtractAll
{
    public class MockExtractorTaskSequencerHandler() : IExtractorTaskSequencerHandler<string, BasicNodeDiscriminator>

    {
        public ExtractorOption ExtractorOption { get; } = new();

        public IStructDocNodeFactoryProvider<string, BasicNodeDiscriminator> DefaultNodeFactoryProvider { get; } = new StructDocNodeFactoryProvider<string, BasicNodeDiscriminator>();

        public delegate ProcessTaskResult<string, BasicNodeDiscriminator> ProcessTaskCallback(TaskContext<string, BasicNodeDiscriminator> taskContext);

        public StringPreviewProvider DataPreviewProvider { get; } = new();

        private readonly Queue<ProcessTaskCallback> _onProcessTaskQueue = [];

        public void AddOnProcessTaskAction(ProcessTaskCallback action)
            => _onProcessTaskQueue.Enqueue(action);

        public bool HasOnProcessTask => _onProcessTaskQueue.Count > 0;


        public IProcessTaskResult<string, BasicNodeDiscriminator> OnProcessTask(TaskContext<string, BasicNodeDiscriminator> taskContext)
        {
            if (!_onProcessTaskQueue.TryDequeue(out ProcessTaskCallback? nextAction))
                throw new InvalidOperationException("No Next action in queue.");

            return nextAction.Invoke(taskContext);
        }

        public IChildrenTaskInfoFactory ChildrenTaskInfoFactory { get; } = Extraction.Factory.ChildrenTaskInfoFactory.Default;

        public string GetDataPreview(string? data)
            => DataPreviewProvider.GetPreview(data);

        public ILogger<TCategoryName> CreateLogger<TCategoryName>() => NullLogger<TCategoryName>.Instance;
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
        MockExtractorTaskSequencerHandler handler = new();
        ExtractorTaskSequencer<string, BasicNodeDiscriminator> extractor = new(handler);

        // When
        ExtractionResult result = extractor.ProcessTasks();

        // Then
        Assert.False(handler.HasOnProcessTask);
        Assert.Empty(result.StructDocNodes);
        Assert.Equal(0, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
    }

    [Fact]
    public void OneTaskGeneratingARoot_OneRootReturned()
    {
        // Given
        MockExtractorTaskSequencerHandler handler = new();
        handler.AddOnProcessTaskAction(ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData)));

        ExtractorTaskSequencer<string, BasicNodeDiscriminator> extractor = new(handler);
        extractor.AddTask(DefaultTaskValue);

        // When
        ExtractionResult result = extractor.ProcessTasks();

        // Then
        Assert.False(handler.HasOnProcessTask);
        Assert.Single(result.StructDocNodes, r => r.Id == DefaultTaskValue);
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
    }

    [Fact]
    public void OneTaskWithTwoSubTask_OneRootWithTwoChildrenReturned()
    {
        // Given
        MockExtractorTaskSequencerHandler handler = new();
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2]));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));

        ExtractorTaskSequencer<string, BasicNodeDiscriminator> extractor = new(handler);
        extractor.AddTask(DefaultTaskValue);

        // When
        ExtractionResult result = extractor.ProcessTasks();

        // Then
        Assert.False(handler.HasOnProcessTask);
        IStructDocNode node = Assert.Single(result.StructDocNodes, r => r.Id == DefaultTaskValue);
        BasicSrcRootBlock root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Collection(root.Content,
            c => Assert.Equal(DefaultTaskValue1, Assert.IsType<BasicSrcNode>(c).Id),
            c => Assert.Equal(DefaultTaskValue2, Assert.IsType<BasicSrcNode>(c).Id));
        Assert.Equal(3, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
    }

    [Fact]
    public void OneTaskWithTwoSubTaskAndExtraTask_TwoRootReturned()
    {
        // Given
        MockExtractorTaskSequencerHandler handler = new();
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2], [DefaultTaskValueA]));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData)));

        ExtractorTaskSequencer<string, BasicNodeDiscriminator> extractor = new(handler);
        extractor.AddTask(DefaultTaskValue);

        // When
        ExtractionResult result = extractor.ProcessTasks();

        // Then
        Assert.False(handler.HasOnProcessTask);
        Assert.Equal(2, result.StructDocNodes.Count());
        var it = result.StructDocNodes.GetEnumerator();
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
        Assert.Equal(4, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
    }

    [Fact]
    public void OneTaskWithTwoSubTaskOneWithoutResult_OneRootWithOneChildrenReturned()
    {
        // Given
        MockExtractorTaskSequencerHandler handler = new();
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcRootBlock(ctx.ExtractionData),
                                 [DefaultTaskValue1, DefaultTaskValue2]));
        handler.AddOnProcessTaskAction(
            ctx => NewTaskResult(new BasicSrcNode(ctx.ExtractionData)));
        handler.AddOnProcessTaskAction(
            ctx => ProcessTaskResult.NewSkipped<string, BasicNodeDiscriminator>());

        ExtractorTaskSequencer<string, BasicNodeDiscriminator> extractor = new(handler);
        extractor.AddTask(DefaultTaskValue);

        // When
        ExtractionResult result = extractor.ProcessTasks();

        // Then
        Assert.False(handler.HasOnProcessTask);
        IStructDocNode node = Assert.Single(result.StructDocNodes, r => r.Id == DefaultTaskValue);
        BasicSrcRootBlock root = Assert.IsType<BasicSrcRootBlock>(node);
        Assert.Single(root.Content, c => Assert.IsType<BasicSrcNode>(c).Id == DefaultTaskValue1);
        Assert.Equal(3, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(1, result.Summery.SkippedTaskCount);
    }
}
