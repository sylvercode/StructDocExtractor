using Microsoft.Extensions.Logging;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

public class ProxyChildrenTaskInfo(ExtractionTask task,
                                   IEnumerable<object>? childrenData,
                                   ILogger<ProxyChildrenTaskInfo>? logger = null)
    : ChildrenTaskInfo(task, childrenData, untypedLogger: logger)
{
    private readonly IChildrenTaskInfo _parentChildrenTaskInfo
        = task.ParentTaskInfo?.ParentTask?.ChildrenTaskInfo
        ?? throw new ArgumentException($"{nameof(task)} has no parent task info with children info.");

    public override void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        base.AddPendingSubTaskIndex(taskIndex);
        _parentChildrenTaskInfo.AddPendingSubTaskIndex(GetIndexForParent(taskIndex));
    }

    private TaskIndex GetIndexForParent(TaskIndex nextTaskIndex)
        => new(Task.ParentTaskInfo!.SiblingSubTaskIndex, nextTaskIndex);

    public override void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        base.OnSubTaskResultSet(taskIndex, subTask);
        _parentChildrenTaskInfo.OnSubTaskResultSet(GetIndexForParent(taskIndex), subTask);
    }
}
