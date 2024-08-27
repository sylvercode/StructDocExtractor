using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ChildrenTaskInfo(ExtractionTask task) : IChildrenTaskInfo
{
    public ExtractionTask Task { get; } = task;

    private readonly List<ExtractionTask> _childrenTasks = [];
    public IReadOnlyList<ExtractionTask> ChildrenTasks => _childrenTasks.AsReadOnly();

    private readonly HashSet<TaskIndex> _pendingSubTaskIndex = [];
    public IReadOnlyList<TaskIndex> PendingSubTaskIndex => _pendingSubTaskIndex.ToList().AsReadOnly();

    public bool HasPendingSubTaskIndex => _pendingSubTaskIndex.Count > 0;

    public ChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData) : this(task)
    {
        TaskIndex nextTaskIndex = new();
        foreach (var data in childrenData ?? [])
        {
            NewChildTask(data, nextTaskIndex);
            nextTaskIndex = nextTaskIndex.Increment();
        }
    }

    protected void NewChildTask(object childData, TaskIndex nextTaskIndex)
    {
        ExtractionTask subTask = new(childData, new ParentTaskInfo(Task, nextTaskIndex));
        _childrenTasks.Add(subTask);

        RegisterChildTaskResultSet(subTask, nextTaskIndex);
    }

    protected virtual void SubTaskResulSetted(ExtractionTask subTask)
    {
        TaskIndex taskIndex = subTask.ParentTaskInfo!.SiblingSubTaskIndex;
        if (!_pendingSubTaskIndex.Remove(taskIndex))
            throw new InvalidOperationException($"Unknow subtask: {taskIndex}");
    }

    protected virtual void RegisterChildTaskResultSet(ExtractionTask task, TaskIndex taskIndex)
    {
        task.ResultSetted += SubTaskResulSetted;
        _pendingSubTaskIndex.Add(taskIndex);
    }

    public virtual void RegisterGrandChildTaskResultSet(ExtractionTask task, TaskIndex taskIndex)
    {
        RegisterChildTaskResultSet(
            task,
            new TaskIndex(Task.ParentTaskInfo?.SiblingSubTaskIndex, taskIndex));
    }
}
