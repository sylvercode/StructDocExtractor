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
        /*
        if (childrenData is not null)
        {
            if (Task.TaskResult?.SrcNode is null)
            {
                if (Task.ParentTaskInfo is not null)
                {
                    TaskIndex myIndex = Task.ParentTaskInfo.SiblingSubTaskIndex;
                    ExtractionTask parentTask = task;
                    for (int i = 0; i < myIndex.Length; i++)
                        parentTask = parentTask.ParentTaskInfo!.ParentTask;
                }
            }
            else
            {

            }
        }
        */
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

        LisentChildTaskResultSet(subTask, nextTaskIndex);
    }

    protected virtual void SubTaskResulSetted(ExtractionTask subTask)
    {
        TaskIndex taskIndex = subTask.ParentTaskInfo!.SiblingSubTaskIndex;
        _pendingSubTaskIndex.Remove(taskIndex);
    }

    protected void LisentChildTaskResultSet(ExtractionTask task, TaskIndex taskIndex)
    {
        task.ResultSetted += SubTaskResulSetted;
        _pendingSubTaskIndex.Add(taskIndex);
    }
}
