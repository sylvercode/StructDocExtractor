using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    public NodeHolderChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData) : base(task)
    {
        if (Task.TaskResult?.SrcNode is not ISrcNodeHolderInitializer holder)
            throw new InvalidOperationException($"{nameof(task)} result node is not {nameof(ISrcNodeHolderInitializer)}");

        ChildLinker = holder.NewParentChildLinkIntializer();

        TaskIndex nextTaskIndex = new();
        foreach (var data in childrenData ?? [])
        {
            NewChildTask(data, nextTaskIndex);
            nextTaskIndex = nextTaskIndex.Increment();
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
