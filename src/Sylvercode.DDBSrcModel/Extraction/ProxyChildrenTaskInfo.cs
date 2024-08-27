namespace Sylvercode.DDBSrcModel.Extraction;

public class ProxyChildrenTaskInfo(
    ExtractionTask task,
    IEnumerable<object>? childrenData) : ChildrenTaskInfo(task, childrenData)
{
    private readonly IChildrenTaskInfo _parentChildrenTaskInfo
        = task.ParentTaskInfo?.ParentTask?.ChildrenTaskInfo
        ?? throw new ArgumentException($"{nameof(task)} has no parent task info whith children info.");

    protected override void RegisterChildTaskResultSet(ExtractionTask task, TaskIndex taskIndex)
    {
        base.RegisterChildTaskResultSet(task, taskIndex);
        _parentChildrenTaskInfo.RegisterGrandChildTaskResultSet(task, taskIndex);
    }
}
