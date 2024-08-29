namespace Sylvercode.DDBSrcModel.Extraction;

public class ProxyChildrenTaskInfo(
    ExtractionTask task,
    IEnumerable<object>? childrenData) : ChildrenTaskInfo(task, childrenData)
{
    private readonly IChildrenTaskInfo _parentChildrenTaskInfo
        = task.ParentTaskInfo?.ParentTask?.ChildrenTaskInfo
        ?? throw new ArgumentException($"{nameof(task)} has no parent task info whith children info.");

    public override void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        base.AddPendingSubTaskIndex(taskIndex);
        _parentChildrenTaskInfo.AddPendingSubTaskIndex(GetIndexForParent(taskIndex));
    }

    private TaskIndex GetIndexForParent(TaskIndex nextTaskIndex) 
        => new(Task.ParentTaskInfo!.SiblingSubTaskIndex, nextTaskIndex);

    public override void OnSubTaskResulSetted(TaskIndex taskIndex, ExtractionTask subTask)
    {
        base.OnSubTaskResulSetted(taskIndex, subTask);
        _parentChildrenTaskInfo.OnSubTaskResulSetted(GetIndexForParent(taskIndex), subTask);
    }
}
