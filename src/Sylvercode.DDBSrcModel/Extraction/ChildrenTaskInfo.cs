using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;


public interface IChildrenTaskInfo
{
    ExtractionTask Task { get; }

    IReadOnlyList<ExtractionTask> ChildrenTasks { get; }

    IReadOnlyList<int[]> PendingSubTaskIndex { get; }

    bool HasPendingSubTaskIndex { get; }
}

public abstract class ChildrenTaskInfo(ExtractionTask task) : IChildrenTaskInfo
{
    private class IntArrayEqualityComparer : IEqualityComparer<int[]>
    {
        public static IntArrayEqualityComparer Default = new();
        public bool Equals(int[]? x, int[]? y)
            => StructuralComparisons.StructuralEqualityComparer.Equals(x, y);

        public int GetHashCode([DisallowNull] int[] obj)
            => StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
    }

    public ExtractionTask Task { get; } = task;

    private readonly List<ExtractionTask> _childrenTasks = [];
    public IReadOnlyList<ExtractionTask> ChildrenTasks => _childrenTasks.AsReadOnly();

    private readonly HashSet<int[]> _pendingSubTaskIndex = [];
    public IReadOnlyList<int[]> PendingSubTaskIndex => _pendingSubTaskIndex.ToList().AsReadOnly();

    public bool HasPendingSubTaskIndex => _pendingSubTaskIndex.Count > 0;

    public ChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData) : this(task)
    {
        if (childrenData is not null)
        {
            if (Task.TaskResult?.SrcNode is null)
            {
                if (Task.ParentTaskInfo is not null)
                {
                    int[] myIndex = Task.ParentTaskInfo.SiblingSubTaskIndex;
                    ExtractionTask parentTask = task;
                    for (int i = 0; i < myIndex.Length; i++)
                        parentTask = parentTask.ParentTaskInfo!.ParentTask;


                }
            }
            else
            {

            }
        }
    }

    protected void NewChildTask(object childData, int[] nextTaskIndex)
    {
        ExtractionTask subTask = new(childData, new ParentTaskInfo(Task, nextTaskIndex));
        _childrenTasks.Add(subTask);

        LisentChildTaskResultSet(subTask, nextTaskIndex);
    }

    protected virtual void SubTaskResulSetted(ExtractionTask subTask)
    {
        int[] taskIndex = subTask.ParentTaskInfo!.SiblingSubTaskIndex;
        _pendingSubTaskIndex.Remove(taskIndex);
    }

    protected void LisentChildTaskResultSet(ExtractionTask task, int[] taskIndex)
    {
        task.ResultSetted += SubTaskResulSetted;
        _pendingSubTaskIndex.Add(taskIndex);
    }
}

public class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    public NodeHolderChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData) : base(task)
    {
        if (Task.TaskResult?.SrcNode is not ISrcNodeHolderInitializer holder)
            throw new InvalidOperationException($"{nameof(task)} result node is not {nameof(ISrcNodeHolderInitializer)}");

        ChildLinker = holder.NewParentChildLinkIntializer();

        int nextTaskIndex = 0;
        foreach (var data in childrenData ?? [])
        {
            NewChildTask(data, [nextTaskIndex]);
            nextTaskIndex++;
        }
    }

    public IParentChildLinkIntializer ChildLinker { get; }

    protected override void SubTaskResulSetted(ExtractionTask subTask)
    {
        base.SubTaskResulSetted(subTask);

        ISrcNode? node = subTask.TaskResult?.SrcNode;
        if (node is ISrcNodeIntializer srcInit
            && !srcInit.IsRoot)
            ChildLinker.AddChild(srcInit);

        if (!HasPendingSubTaskIndex)
            ChildLinker.InitializeParentChildLink();
    }
}
