using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTask(
    object extractionData,
    ParentTaskInfo? parentTaskInfo = null,
    ILogger? logger = null)
    : IExtractionTask
{
    protected ILogger Logger { get; } = logger ?? NullLogger.Instance;

    public object ExtractionData { get; } = extractionData;
    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;
    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }
    public ExtractionTaskResult? TaskResult { get; private set; }

    public event IExtractionTask.ResultSettedEventHandler? ResultSetted;
    protected void OnResultSetted() => ResultSetted?.Invoke(this);

    public void SetResult(
        IProcessTaskResult processTaskResult,
        IChildrenTaskInfoFactory childrenTaskInfoFactory)
    {
        TaskResult = new ExtractionTaskResult(processTaskResult);

        ChildrenTaskInfo = childrenTaskInfoFactory.NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSetted();
    }
}
