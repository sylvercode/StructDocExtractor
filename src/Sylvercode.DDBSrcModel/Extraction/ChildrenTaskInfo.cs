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
        subTask.ResultSet += OnChildTaskResultSet;
        _childrenTasks.Add(subTask);
        AddPendingSubTaskIndex(nextTaskIndex);
    }

    public virtual void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        if (!_pendingSubTaskIndex.Add(taskIndex))
            throw new InvalidOperationException($"Duplicated subtask: {taskIndex}");
    }

    public virtual void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        if (!_pendingSubTaskIndex.Remove(taskIndex))
            throw new InvalidOperationException($"Unknown subtask: {taskIndex}");
    }

    protected virtual void OnChildTaskResultSet(ExtractionTask subTask)
        => OnSubTaskResultSet(subTask.ParentTaskInfo!.SiblingSubTaskIndex, subTask);
}
