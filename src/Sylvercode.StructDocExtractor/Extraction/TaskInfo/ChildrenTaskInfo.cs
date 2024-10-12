using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

public partial class ChildrenTaskInfo : IChildrenTaskInfo
{
    private readonly ILogger _logger;
    protected ILogger Logger => _logger;

    public ExtractionTask Task { get; }

    private readonly List<ExtractionTask> _childrenTasks = [];
    public IReadOnlyList<ExtractionTask> ChildrenTasks => _childrenTasks.AsReadOnly();

    private readonly HashSet<TaskIndex> _pendingSubTaskIndex = [];
    public IReadOnlyList<TaskIndex> PendingSubTaskIndex => _pendingSubTaskIndex.ToList().AsReadOnly();

    public bool HasPendingSubTaskIndex => _pendingSubTaskIndex.Count > 0;

    public ChildrenTaskInfo(ExtractionTask task,
                            IEnumerable<object>? childrenData,
                            ILogger<ChildrenTaskInfo>? logger = null)
        : this(task, childrenData, untypedLogger: logger)
    {
    }

    protected ChildrenTaskInfo(ExtractionTask task,
                               IEnumerable<object>? childrenData,
                               ILogger? untypedLogger = null)
    {
        Task = task;
        _logger = untypedLogger ?? NullLogger.Instance;

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
        subTask.ResultSet += OnChildTaskResultSet;
        _childrenTasks.Add(subTask);
        AddPendingSubTaskIndex(nextTaskIndex);
    }

    public virtual void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        if (!_pendingSubTaskIndex.Add(taskIndex))
            throw new InvalidOperationException($"Duplicated subtask: {taskIndex}");

        if (_logger.IsEnabled(LogLevel.Trace))
            LogTaskAdded(Task.TaskSnippet(), taskIndex);
    }

    public virtual void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        if (!_pendingSubTaskIndex.Remove(taskIndex))
            throw new InvalidOperationException($"Unknown subtask: {taskIndex}");

        if (_logger.IsEnabled(LogLevel.Trace))
            LogTaskRemoved(Task.TaskSnippet(), taskIndex);
    }

    protected virtual void OnChildTaskResultSet(object? sender, EventArgs e)
    {
        if (sender is not ExtractionTask subTask)
            throw new InvalidOperationException($"Invalid sender: {sender}");

        OnSubTaskResultSet(subTask.ParentTaskInfo!.SiblingSubTaskIndex, subTask);
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "{TaskSnippet}: removed subtask {subTaskIndex}.",
        SkipEnabledCheck = true)]
    private partial void LogTaskRemoved(string taskSnippet, TaskIndex subTaskIndex);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "{TaskSnippet}: added subtask {subTaskIndex}.",
        SkipEnabledCheck = true)]
    private partial void LogTaskAdded(string taskSnippet, TaskIndex subTaskIndex);
}
