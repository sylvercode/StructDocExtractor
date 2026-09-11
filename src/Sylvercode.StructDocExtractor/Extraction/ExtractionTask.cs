using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Concrete implementation of <see cref="IExtractionTask"/> carrying source data, parent context, and extraction result for a single pipeline step.</summary>
/// <param name="extractionData">The source data element this task is responsible for extracting.</param>
/// <param name="parentTaskInfo">Optional parent-context descriptor linking this task to its parent task.</param>
/// <param name="logger">Optional logger instance; falls back to a null logger when omitted.</param>
/// <remarks>
/// Wires child-task creation via <see cref="ChildrenTaskInfoFactory"/> when <see cref="SetResult"/> is called,
/// and raises <see cref="ResultSet"/> to notify sequencer observers.
/// </remarks>
public partial class ExtractionTask(
    object extractionData,
    ParentTaskInfo? parentTaskInfo = null,
    ILogger<ExtractionTask>? logger = null)
    : IExtractionTask
{
    private readonly ChildrenTaskInfoFactory _childrenTaskInfoFactory = ChildrenTaskInfoFactory.Default;
    protected ILogger<ExtractionTask> Logger { get; } = logger ?? NullLogger<ExtractionTask>.Instance;

    /// <summary>Gets the source data element this task is responsible for extracting.</summary>
    public object ExtractionData { get; } = extractionData;
    /// <summary>Gets the parent-context descriptor, or <see langword="null"/> if this is a root task.</summary>
    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;
    /// <summary>Gets the child-task descriptor populated when <see cref="SetResult"/> is called.</summary>
    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }
    /// <summary>Gets the extraction result set by the sequencer handler, or <see langword="null"/> before the task is processed.</summary>
    public ExtractionTaskResult? TaskResult { get; private set; }

    /// <summary>Raised after <see cref="SetResult"/> is called and the task result has been stored.</summary>
    public event EventHandler? ResultSet;
    protected void OnResultSet() => ResultSet?.Invoke(this, EventArgs.Empty);

    /// <summary>Records the process result, creates child tasks, and raises <see cref="ResultSet"/>.</summary>
    /// <param name="processTaskResult">The task-processing outcome produced by the sequencer handler.</param>
    public void SetResult(
        IProcessTaskResult processTaskResult)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
        {
            string taskSnippet = TaskSnippet(withParentSnippet: true);
            LogSetResult(Logger, taskSnippet);
        }

        TaskResult = new ExtractionTaskResult(processTaskResult);

        ChildrenTaskInfo = _childrenTaskInfoFactory.NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSet();
    }

    /// <summary>Returns a short diagnostic string identifying this task within its sibling sequence.</summary>
    /// <param name="withParentSnippet">When <see langword="true"/>, appends the parent task's snippet for full chain context.</param>
    /// <returns>A human-readable snippet string, or <c>"&lt;&lt;root&gt;&gt;"</c> for root tasks with no discriminator.</returns>
    public string TaskSnippet(bool withParentSnippet = false)
    {
        var snippet = TaskResult?.DataDiscriminator?.ToString() ?? string.Empty;
        if (ParentTaskInfo is null)
            return string.IsNullOrEmpty(snippet) ? "<<root>>" : snippet;

        var index = ParentTaskInfo.SiblingSubTaskIndex.ToString();
        if (!string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(snippet))
            snippet += $"{index}:[{snippet}]";

        if (withParentSnippet)
        {
            var parentSnippet = ParentTaskInfo.ParentTask.TaskSnippet();
            if (!string.IsNullOrEmpty(parentSnippet))
                snippet = $"{snippet} of {parentSnippet}";
        }

        return snippet;
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Set Result for `{TaskSnippet}`.",
        SkipEnabledCheck = true)]
    private static partial void LogSetResult(ILogger logger, string taskSnippet);
}
