using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Default implementation of <see cref="IChildrenTaskInfoFactory"/> that selects the appropriate <see cref="IChildrenTaskInfo"/> variant based on the task's result node type and parent context.</summary>
/// <param name="loggerFactory">Optional logger factory for logging task-info creation decisions; falls back to a null factory when omitted.</param>
/// <remarks>
/// Uses <see cref="NodeHolderChildrenTaskInfo"/> when the result implements <see cref="IStructDocNodeHolderInitializer"/>,
/// <see cref="ProxyChildrenTaskInfo"/> when the result is null but an ancestor holds a node holder, and
/// <see cref="ChildrenTaskInfo"/> otherwise.
/// </remarks>
public partial class ChildrenTaskInfoFactory(
    ILoggerFactory? loggerFactory = null) : IChildrenTaskInfoFactory
{

    /// <summary>Gets a shared singleton instance with no logging.</summary>
    public static ChildrenTaskInfoFactory Default { get; } = new();

    private readonly ILogger _logger = loggerFactory?.CreateLogger<ChildrenTaskInfoFactory>()
                                       ?? NullLogger<ChildrenTaskInfoFactory>.Instance;

    private readonly ILogger<NodeHolderChildrenTaskInfo> _nodeHolderChildrenTaskInfoLogger =
        loggerFactory?.CreateLogger<NodeHolderChildrenTaskInfo>()
        ?? NullLogger<NodeHolderChildrenTaskInfo>.Instance;

    private readonly ILogger<ProxyChildrenTaskInfo> _proxyChildrenTaskInfoLogger =
        loggerFactory?.CreateLogger<ProxyChildrenTaskInfo>()
        ?? NullLogger<ProxyChildrenTaskInfo>.Instance;

    private readonly ILogger<ChildrenTaskInfo> _childrenTaskInfoLogger =
        loggerFactory?.CreateLogger<ChildrenTaskInfo>()
        ?? NullLogger<ChildrenTaskInfo>.Instance;

    /// <inheritdoc/>
    public IChildrenTaskInfo NewChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData)
    {

        IStructDocNode? resultNode = task.TaskResult?.SrcNode;
        if (resultNode is IStructDocNodeHolderInitializer)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                string taskSnippet = task.TaskSnippet();
                LogNewChildrenTaskInfo(_logger, nameof(NodeHolderChildrenTaskInfo), taskSnippet);
            }

            return new NodeHolderChildrenTaskInfo(task, childrenData, _nodeHolderChildrenTaskInfoLogger);
        }

        if (resultNode is null
            && HasParentNodeHolder(task))
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                string taskSnippet = task.TaskSnippet();
                LogNewChildrenTaskInfo(_logger, nameof(ProxyChildrenTaskInfo), taskSnippet);
            }

            return new ProxyChildrenTaskInfo(task, childrenData, _proxyChildrenTaskInfoLogger);
        }

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            string taskSnippet = task.TaskSnippet();
            LogNewChildrenTaskInfo(_logger, nameof(ChildrenTaskInfo), taskSnippet);
        }

        return new ChildrenTaskInfo(task, childrenData, _childrenTaskInfoLogger);
    }

    private static bool HasParentNodeHolder(ExtractionTask task)
    {
        ExtractionTask? parentTask = task.ParentTaskInfo?.ParentTask;
        while (parentTask is not null)
        {
            if (parentTask.TaskResult?.SrcNode is IStructDocNodeHolderInitializer)
                return true;
            parentTask = parentTask.ParentTaskInfo?.ParentTask;
        }

        return false;
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "New {TypeName} for `{TaskSnippet}`.")]
    private static partial void LogNewChildrenTaskInfo(ILogger logger, string typeName, string taskSnippet);
}
