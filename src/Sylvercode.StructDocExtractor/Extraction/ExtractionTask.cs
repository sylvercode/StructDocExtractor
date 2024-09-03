using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.StructDocExtractor.Extraction;

public partial class ExtractionTask(
    object extractionData,
    ParentTaskInfo? parentTaskInfo = null,
    ILogger<ExtractionTask>? logger = null)
    : IExtractionTask
{
    protected ILogger<ExtractionTask> Logger { get; } = logger ?? NullLogger<ExtractionTask>.Instance;

    public object ExtractionData { get; } = extractionData;
    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;
    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }
    public ExtractionTaskResult? TaskResult { get; private set; }

    public event EventHandler? ResultSet;
    protected void OnResultSet() => ResultSet?.Invoke(this, EventArgs.Empty);

    public void SetResult(
        IProcessTaskResult processTaskResult,
        IChildrenTaskInfoFactory childrenTaskInfoFactory)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
            LogSetResult(Logger, TaskSnippet(true)); ;

        TaskResult = new ExtractionTaskResult(processTaskResult);

        ChildrenTaskInfo = childrenTaskInfoFactory.NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSet();
    }

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
