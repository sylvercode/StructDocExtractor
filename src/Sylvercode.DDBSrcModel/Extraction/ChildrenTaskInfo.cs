using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ChildrenTaskInfo(ExtractionTask task)
{
    public ExtractionTask Task { get; } = task;
    public IParentChildLinkIntializer? ChildLinker { get; }
    private readonly List<ExtractionTask> _childrenTasks = [];
    public IReadOnlyList<ExtractionTask> ChildrenTasks => _childrenTasks.AsReadOnly();
    private readonly HashSet<int> _pendingSubTaskIndex = [];
    public IReadOnlyList<int> PendingSubTaskIndex => _pendingSubTaskIndex.ToList().AsReadOnly();

    public ChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData) : this(task)
    {
        if (Task.TaskResult?.SrcNode is ISrcNodeHolderInitializer holder)
            ChildLinker = holder.NewParentChildLinkIntializer();

        int nextTaskIndex = 0;
        foreach (var data in childrenData ?? [])
        {
            NewChildTask(data, nextTaskIndex);
            nextTaskIndex++;
        }
    }

    private void NewChildTask(object childData, int nextTaskIndex)
    {
        ExtractionTask subTask = new(childData, new ParentTaskInfo(Task, nextTaskIndex));
        subTask.ResultSetted += SubTaskResulSetted;

        _childrenTasks.Add(subTask);
        _pendingSubTaskIndex.Add(nextTaskIndex);
    }

    private void SubTaskResulSetted(ExtractionTask subTask)
    {
        int taskIndex = subTask.ParentTaskInfo!.SiblingSubTaskIndex;
        _pendingSubTaskIndex.Remove(taskIndex);

        ISrcNode? node = subTask.TaskResult?.SrcNode;
        if (node is ISrcNodeIntializer srcInit
            && (!node?.IsRoot ?? false))
            ChildLinker?.AddChild(srcInit);

        if (ChildLinker is not null
            && _pendingSubTaskIndex.Count == 0)
            ChildLinker.InitializeParentChildLink();
    }
}
