namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTask(object extractionData, ParentTaskInfo? parentTaskInfo = null)
{
    public class ResultSetsEventArgs(ExtractionTask task)
    {
        public ExtractionTask Task => task;
    }

    public object ExtractionData { get; } = extractionData;

    public ParentTaskInfo? ParentTaskInfo { get; } = parentTaskInfo;

    public ChildrenTaskInfo? ChildrenTaskInfo { get; private set; }

    public ExtractionTaskResult? TaskResult { get; private set; }

    public delegate void ResultSetsEventHandler(ExtractionTask sender);
    public event ResultSetsEventHandler? ResultSets;
    protected void OnResultSets() => ResultSets?.Invoke(this);

    public void ProcessResult(IProcessTaskResult processTaskResult)
    {
        TaskResult = new(processTaskResult);

        ChildrenTaskInfo = new(this, processTaskResult.SubTasksExtractionData);

        OnResultSets();
    }
}
