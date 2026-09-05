using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Specialization of <see cref="ChildrenTaskInfo"/> that additionally wires the extracted child nodes into their parent node holder once all child tasks have completed.</summary>
/// <remarks>
/// On construction, obtains an <see cref="IParentChildLinkInitializer"/> from the parent node's
/// <see cref="IStructDocNodeHolderInitializer"/> and accumulates each completed child node via
/// <see cref="IParentChildLinkInitializer.AddChild"/>. When the last pending subtask completes,
/// <see cref="IParentChildLinkInitializer.InitializeParentChildLink"/> is called exactly once,
/// atomically setting the parent-child relationships for the whole subtree.
/// </remarks>
public partial class NodeHolderChildrenTaskInfo : ChildrenTaskInfo
{
    /// <summary>Initializes a new instance of <see cref="NodeHolderChildrenTaskInfo"/>, creating child tasks for each item in <paramref name="childrenData"/> and obtaining the child linker from the parent node.</summary>
    /// <param name="task">The completed parent extraction task whose result node is the holder.</param>
    /// <param name="childrenData">The source data items to extract as children; may be <see langword="null"/> for zero children.</param>
    /// <param name="logger">An optional typed logger for task lifecycle diagnostics.</param>
    /// <exception cref="InvalidOperationException">Thrown when the parent task's result node does not implement <see cref="IStructDocNodeHolderInitializer"/>.</exception>
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

    /// <summary>Gets the parent-child link initializer used to accumulate and wire extracted child nodes into the parent holder.</summary>
    public IParentChildLinkInitializer ChildLinker { get; }

    /// <inheritdoc/>
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
