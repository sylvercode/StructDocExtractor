using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Default implementation of <see cref="IChildrenTaskInfo"/> that spawns and tracks child extraction tasks from a parent task's result.</summary>
/// <remarks>
/// Manages the lifecycle of child extraction tasks by assigning each a sequential <see cref="TaskIndex"/>,
/// subscribing to their <c>ResultSet</c> events, and maintaining a set of pending indices that shrinks as
/// tasks complete. Duplicate or unknown subtask indices throw <see cref="InvalidOperationException"/> to
/// prevent silent tracking inconsistencies.
/// </remarks>
public partial class ChildrenTaskInfo : IChildrenTaskInfo
{
    private readonly ILogger _logger;
    /// <summary>Gets the logger used for trace-level task lifecycle diagnostics.</summary>
    protected ILogger Logger => _logger;

    /// <inheritdoc/>
    public ExtractionTask Task { get; }

    private readonly List<ExtractionTask> _childrenTasks = [];
    /// <inheritdoc/>
    public IReadOnlyList<ExtractionTask> ChildrenTasks => _childrenTasks.AsReadOnly();

    private readonly HashSet<TaskIndex> _pendingSubTaskIndex = [];
    /// <inheritdoc/>
    public IReadOnlyList<TaskIndex> PendingSubTaskIndex => _pendingSubTaskIndex.ToList().AsReadOnly();

    /// <inheritdoc/>
    public bool HasPendingSubTaskIndex => _pendingSubTaskIndex.Count > 0;

    /// <summary>Initializes a new instance of <see cref="ChildrenTaskInfo"/> for <paramref name="task"/>, creating a child task for each item in <paramref name="childrenData"/>.</summary>
    /// <param name="task">The parent extraction task that owns these child tasks.</param>
    /// <param name="childrenData">The source data items from which to create child tasks; may be <see langword="null"/> for zero children.</param>
    /// <param name="logger">An optional typed logger for task lifecycle diagnostics.</param>
    public ChildrenTaskInfo(ExtractionTask task,
                            IEnumerable<object>? childrenData,
                            ILogger<ChildrenTaskInfo>? logger = null)
        : this(task, childrenData, untypedLogger: logger)
    {
    }

    /// <summary>Initializes a new instance of <see cref="ChildrenTaskInfo"/> with an untyped logger, intended for subclass constructors.</summary>
    /// <param name="task">The parent extraction task that owns these child tasks.</param>
    /// <param name="childrenData">The source data items from which to create child tasks; may be <see langword="null"/> for zero children.</param>
    /// <param name="untypedLogger">An optional untyped logger; falls back to <see cref="NullLogger.Instance"/> if <see langword="null"/>.</param>
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

    /// <summary>Creates a single child <see cref="ExtractionTask"/> for <paramref name="childData"/>, registers it as pending, and subscribes to its result event.</summary>
    /// <param name="childData">The source data for the child task.</param>
    /// <param name="nextTaskIndex">The <see cref="TaskIndex"/> to assign to the new child task.</param>
    protected void NewChildTask(object childData, TaskIndex nextTaskIndex)
    {
        ExtractionTask subTask = new(childData, new ParentTaskInfo(Task, nextTaskIndex));
        subTask.ResultSet += OnChildTaskResultSet;
        _childrenTasks.Add(subTask);
        AddPendingSubTaskIndex(nextTaskIndex);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="taskIndex"/> is already registered as pending.</exception>
    public virtual void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        if (!_pendingSubTaskIndex.Add(taskIndex))
            throw new InvalidOperationException($"Duplicated subtask: {taskIndex}");

        if (_logger.IsEnabled(LogLevel.Trace))
            LogTaskAdded(Task.TaskSnippet(), taskIndex);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="taskIndex"/> is not in the pending set.</exception>
    public virtual void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        if (!_pendingSubTaskIndex.Remove(taskIndex))
            throw new InvalidOperationException($"Unknown subtask: {taskIndex}");

        if (_logger.IsEnabled(LogLevel.Trace))
            LogTaskRemoved(Task.TaskSnippet(), taskIndex);
    }

    /// <summary>Handles the <c>ResultSet</c> event of a child task by forwarding to <see cref="OnSubTaskResultSet"/>.</summary>
    /// <param name="sender">The <see cref="ExtractionTask"/> that raised the event.</param>
    /// <param name="e">Event arguments (unused).</param>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="sender"/> is not an <see cref="ExtractionTask"/>.</exception>
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
