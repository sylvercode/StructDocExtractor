using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class ProxyChildrenTaskInfoTests_ctor
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult, ChildrenTaskInfoFactory.Default);
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
    public void ParentWithChildInfo_NoThrow()
    {
        // Given
        ExtractionTask parentTask = NewTask(NewTaskResult(new BasicSrcRootBlock(DefaultTaskValue), [DefaultTaskValue1]));
        ExtractionTask childTask = parentTask.ChildrenTaskInfo!.ChildrenTasks[0];

        // When-Then: No Throw
        new ProxyChildrenTaskInfo(childTask, []);
    }

    [Fact]
    public void ParentWithChildInfo_Throw()
    {
        // Given
        ExtractionTask parentTask = NewTask();
        ExtractionTask childTask = new(DefaultTaskValue1, new ParentTaskInfo(parentTask, 0));

        // When-Then
        Assert.Throws<ArgumentException>(() =>
            new ProxyChildrenTaskInfo(childTask, []));
    }

}

public class ProxyChildrenTaskInfoTests_RegisterChildTaskResultSet
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(
            string extractionData = DefaultTaskValue,
            ParentTaskInfo? parentTaskInfo = null,
            BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(extractionData, parentTaskInfo);
        if (taskResult is not null)
            result.ProcessResult(taskResult, ChildrenTaskInfoFactory.Default);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(ISrcNode? node, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node);
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        return taskResult;
    }

    [Fact]
    public void ParrentTaskWithEmediateProxy_TaskIndexInParent()
    {
        // Given
        ExtractionTask parentTask = NewTask(taskResult: NewTaskResult(new BasicSrcRootBlock(DefaultTaskValue), [DefaultTaskValue1]));
        ExtractionTask childTask = parentTask.ChildrenTaskInfo!.ChildrenTasks[0];

        // When
        childTask.ProcessResult(NewTaskResult(null, [DefaultTaskValue2]), ChildrenTaskInfoFactory.Default);

        // Then
        Assert.IsType<ProxyChildrenTaskInfo>(childTask.ChildrenTaskInfo);
        Assert.NotNull(parentTask.ChildrenTaskInfo);
        Assert.Collection(parentTask.ChildrenTaskInfo.PendingSubTaskIndex,
                          i => Assert.Equal([0, 0], i));
    }
}
