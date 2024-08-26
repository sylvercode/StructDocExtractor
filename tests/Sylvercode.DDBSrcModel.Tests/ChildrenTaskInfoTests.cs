using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class ChildrenTaskInfoTests_ctor
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(ISrcNode node, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node);
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        return taskResult;
    }

    [Fact]
    public void WithAParentNotHolder_InitNoLinker()
    {
        // Given
        ExtractionTask task = NewTask(NewTaskResult(new BasicSrcNode()));

        // When
        NodeHolderChildrenTaskInfo result = new(task, []);

        // Then
        Assert.Equal(result.Task, task);
        Assert.Null(result.ChildLinker);
        Assert.Empty(result.ChildrenTasks);
        Assert.Empty(result.PendingSubTaskIndex);
    }

    [Fact]
    public void WithAParentHolder_InitALinker()
    {
        // Given
        ExtractionTask task = NewTask(NewTaskResult(new BasicSrcBloc()));

        // When
        NodeHolderChildrenTaskInfo result = new(task, []);

        // Then
        Assert.Equal(result.Task, task);
        Assert.NotNull(result.ChildLinker);
        Assert.Empty(result.ChildrenTasks);
        Assert.Empty(result.PendingSubTaskIndex);
    }

    [Fact]
    public void WithOneChildData_InitWithOneChildInfo()
    {
        // Given
        ExtractionTask task = NewTask(NewTaskResult(new BasicSrcBloc()));

        // When
        NodeHolderChildrenTaskInfo result = new(task, [DefaultTaskValue1]);

        // Then
        var childTask = Assert.Single(result.ChildrenTasks);
        Assert.Equal(DefaultTaskValue1, childTask.ExtractionData);
        Assert.Single(result.PendingSubTaskIndex, 0);
    }

    [Fact]
    public void WithTwoChildData_InitWithThwoChildInfo()
    {
        // Given
        ExtractionTask task = NewTask(NewTaskResult(new BasicSrcBloc()));

        // When
        NodeHolderChildrenTaskInfo result = new(task, [DefaultTaskValue1, DefaultTaskValue2]);

        // Then
        Assert.Collection(result.ChildrenTasks,
            t => Assert.Equal(DefaultTaskValue1, t.ExtractionData),
            t => Assert.Equal(DefaultTaskValue2, t.ExtractionData));
        Assert.Equal(2, result.PendingSubTaskIndex.Count);
        Assert.Contains([0], result.PendingSubTaskIndex);
        Assert.Contains([1], result.PendingSubTaskIndex);
    }
}

public class ChildrenTaskInfoTests_SubTaskResulSetted
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(ISrcNode node, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node);
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        return taskResult;
    }

    [Fact]
    public void WithOneOfTwoChildTaskResultSetted_PendingIdxRemovedChildAddToLinker()
    {
        // Given
        ExtractionTask parentTask = NewTask(NewTaskResult(new BasicSrcBloc()));
        NodeHolderChildrenTaskInfo childrenTaskInfo = new(parentTask, [DefaultTaskValue1, DefaultTaskValue2]);
        ExtractionTask childTask = childrenTaskInfo.ChildrenTasks[0];
        BasicSrcNode resultNode = new();

        // When
        childTask.ProcessResult(NewTaskResult(resultNode));

        // Then
        Assert.Single(childrenTaskInfo.PendingSubTaskIndex, 1);
        Assert.Single(childrenTaskInfo.ChildLinker!.ChildrenToAdd(), resultNode);
    }

    [Fact]
    public void WithOneOfOneChildTaskResultSetted_PendingIdxEmptyParentChildLinked()
    {
        // Given
        BasicSrcBloc parentNode = new BasicSrcBloc();
        ExtractionTask parentTask = NewTask(NewTaskResult(parentNode));
        NodeHolderChildrenTaskInfo childrenTaskInfo = new(parentTask, [DefaultTaskValue1]);
        ExtractionTask childTask = childrenTaskInfo.ChildrenTasks[0];
        BasicSrcNode resultNode = new();

        // When
        childTask.ProcessResult(NewTaskResult(resultNode));

        // Then
        Assert.Empty(childrenTaskInfo.PendingSubTaskIndex);
        Assert.Empty(childrenTaskInfo.ChildLinker!.ChildrenToAdd());
        Assert.Equal(parentNode, resultNode.Parent);
        Assert.Single(parentNode.Content, resultNode);
    }
}
