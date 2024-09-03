using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Extraction;

public partial class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    public NodeHolderChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData,
        ILogger<NodeHolderChildrenTaskInfo>? logger = null)
        : base(task, childrenData, untypedLogger: logger)
    {
        if (Task.TaskResult?.SrcNode is not IStructDocNodeHolderInitializer holder)
            throw new InvalidOperationException($"{nameof(task)} result node is not {nameof(IStructDocNodeHolderInitializer)}");

        ChildLinker = holder.NewParentChildLinkInitializer();
    }

    public IParentChildLinkInitializer ChildLinker { get; }

    public override void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        base.OnSubTaskResultSet(taskIndex, subTask);

        IStructDocNode? node = subTask.TaskResult?.SrcNode;
        if (node is IStructDocNodeInitializer srcInit
            && !srcInit.IsRoot)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                LogNodeAdded(Logger, Task.TaskResult!.SrcNode!.NodeSnippet(), node.NodeSnippet());
            ChildLinker.AddChild(srcInit);
        }

        if (!HasPendingSubTaskIndex)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                LogPendingTasksCompleted(Logger, Task.TaskResult!.SrcNode!.NodeSnippet());
            ChildLinker.InitializeParentChildLink();
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Task `{nodeSnippet}` added child `{ChildSnippet}`.",
        SkipEnabledCheck = true)]
    private static partial void LogNodeAdded(ILogger logger, string nodeSnippet, string childSnippet);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "{nodeSnippet} pending node completed.")]
    private static partial void LogPendingTasksCompleted(ILogger logger, string nodeSnippet);
}
