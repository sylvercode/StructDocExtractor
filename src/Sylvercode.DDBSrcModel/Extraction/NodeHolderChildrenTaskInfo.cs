using Microsoft.Extensions.Logging;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    public NodeHolderChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData,
        ILogger<NodeHolderChildrenTaskInfo> logger)
        : base(task, childrenData, untypedLogger:logger)
    {
        if (Task.TaskResult?.SrcNode is not ISrcNodeHolderInitializer holder)
            throw new InvalidOperationException($"{nameof(task)} result node is not {nameof(ISrcNodeHolderInitializer)}");

        ChildLinker = holder.NewParentChildLinkIntializer();
    }

    public IParentChildLinkIntializer ChildLinker { get; }

    public override void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        base.OnSubTaskResultSet(taskIndex, subTask);

        ISrcNode? node = subTask.TaskResult?.SrcNode;
        if (node is ISrcNodeIntializer srcInit
            && !srcInit.IsRoot)
            ChildLinker.AddChild(srcInit);

        if (!HasPendingSubTaskIndex)
            ChildLinker.InitializeParentChildLink();
    }
}
