using Microsoft.Extensions.Logging;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public partial class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    public NodeHolderChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData,
        ILogger<NodeHolderChildrenTaskInfo>? logger = null)
        : base(task, childrenData, untypedLogger: logger)
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
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                LogNodeAdded(Task.TaskResult!.SrcNode!.NodeSnippet(), node.NodeSnippet());
            ChildLinker.AddChild(srcInit);
        }

        if (!HasPendingSubTaskIndex)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                LogPendingTasksCompleted(Task.TaskResult!.SrcNode!.NodeSnippet());
            ChildLinker.InitializeParentChildLink();
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Task `{nodeSnippet}` added child `{ChildSnippet}`.",
        SkipEnabledCheck = true)]
    private partial void LogNodeAdded(string nodeSnippet, string childSnippet);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "{nodeSnippet} pending node completed.")]
    private partial void LogPendingTasksCompleted(string nodeSnippet);
}
