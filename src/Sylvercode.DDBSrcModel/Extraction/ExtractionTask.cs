namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTask(
    object extractionData,
    ParentTaskInfo? parentTaskInfo = null)
    : IExtractionTask
{
    public object ExtractionData { get; } = extractionData;
    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;
    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }
    public ExtractionTaskResult? TaskResult { get; private set; }

    public event IExtractionTask.ResultSettedEventHandler? ResultSetted;
    protected void OnResultSetted() => ResultSetted?.Invoke(this);

    public void SetResult(
        IProcessTaskResult? processTaskResult,
        IChildrenTaskInfoFactory childrenTaskInfoFactory)
    {
        TaskResult = processTaskResult is not null ? new ExtractionTaskResult(processTaskResult)
                                                   : new ExtractionTaskResult();

        ChildrenTaskInfo = childrenTaskInfoFactory.NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSetted();
    }
}
