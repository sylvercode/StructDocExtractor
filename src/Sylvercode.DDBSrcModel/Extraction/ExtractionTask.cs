using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.DDBSrcModel.Extraction;

public partial class ExtractionTask(
    object extractionData,
    ParentTaskInfo? parentTaskInfo = null,
    ILogger<ExtractionTask>? logger = null)
    : IExtractionTask
{
    protected readonly ILogger<ExtractionTask> logger = logger ?? NullLogger<ExtractionTask>.Instance;

    public object ExtractionData { get; } = extractionData;
    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;
    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }
    public ExtractionTaskResult? TaskResult { get; private set; }

    public event IExtractionTask.ResultSetEventHandler? ResultSet;
    protected void OnResultSet() => ResultSet?.Invoke(this);

    public void SetResult(
        IProcessTaskResult processTaskResult,
        IChildrenTaskInfoFactory childrenTaskInfoFactory)
    {
        if (logger.IsEnabled(LogLevel.Trace))
            LogSetResult(TaskSnippet(true)); ;

        TaskResult = new ExtractionTaskResult(processTaskResult);

        ChildrenTaskInfo = childrenTaskInfoFactory.NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSet();
    }

    public string TaskSnippet(bool withParentSnippet = false)
    {
        var snippet = TaskResult?.DataSelectable?.ToString() ?? string.Empty;
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
    private partial void LogSetResult(string taskSnippet);
}
