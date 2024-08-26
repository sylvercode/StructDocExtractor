namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTask(object extractionData, ParentTaskInfo? parentTaskInfo = null)
{
    public class ResultSetsEventArgs(ExtractionTask task)
    {
        public ExtractionTask Task => task;
    }

    public object ExtractionData { get; } = extractionData;

    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;

    public IChildrenTaskInfo? ChildrenTaskInfo { get; private set; }

    public ExtractionTaskResult? TaskResult { get; private set; }

    public delegate void ResultSettedEventHandler(ExtractionTask sender);
    public event ResultSettedEventHandler? ResultSetted;
    protected void OnResultSetted() => ResultSetted?.Invoke(this);

    public void ProcessResult(IProcessTaskResult? processTaskResult)
    {
        if (processTaskResult is not null)
            TaskResult = new(processTaskResult);

        ChildrenTaskInfo = NewChildrenTaskInfo(this, processTaskResult?.SubTasksExtractionData);

        OnResultSetted();
    }

    private IChildrenTaskInfo NewChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData)
    {
        return new NodeHolderChildrenTaskInfo(task, childrenData);
    }
}
