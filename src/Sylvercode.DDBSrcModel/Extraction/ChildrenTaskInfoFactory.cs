using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public partial class ChildrenTaskInfoFactory(
    ILoggerFactory? loggerFactory = null) : IChildrenTaskInfoFactory
{

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

    public IChildrenTaskInfo NewChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData)
    {

        ISrcNode? resultNode = task.TaskResult?.SrcNode;
        if (resultNode is ISrcNodeHolderInitializer)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                LogNewChildrenTaskInfo(_logger, nameof(NodeHolderChildrenTaskInfo), task.TaskSnippet());

            return new NodeHolderChildrenTaskInfo(task, childrenData, _nodeHolderChildrenTaskInfoLogger);
        }

        if (resultNode is null
            && HasParentNodeHolder(task))
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                LogNewChildrenTaskInfo(_logger, nameof(ProxyChildrenTaskInfo), task.TaskSnippet());

            return new ProxyChildrenTaskInfo(task, childrenData, _proxyChildrenTaskInfoLogger);
        }

        if (_logger.IsEnabled(LogLevel.Trace))
            LogNewChildrenTaskInfo(_logger, nameof(ChildrenTaskInfo), task.TaskSnippet());

        return new ChildrenTaskInfo(task, childrenData, _childrenTaskInfoLogger);
    }

    private static bool HasParentNodeHolder(ExtractionTask task)
    {
        ExtractionTask? parentTask = task.ParentTaskInfo?.ParentTask;
        while (parentTask is not null)
        {
            if (parentTask.TaskResult?.SrcNode is ISrcNodeHolderInitializer)
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
