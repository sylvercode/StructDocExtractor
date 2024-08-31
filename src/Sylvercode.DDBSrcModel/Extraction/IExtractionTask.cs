namespace Sylvercode.DDBSrcModel.Extraction;

public interface IExtractionTask
{
    object ExtractionData { get; }
    ParentTaskInfo? ParentTaskInfo { get; }
    IChildrenTaskInfo? ChildrenTaskInfo { get; }
    ExtractionTaskResult? TaskResult { get; }

    public delegate void ResultSettedEventHandler(ExtractionTask sender);
    event ResultSettedEventHandler? ResultSetted;

    void SetResult(IProcessTaskResult processTaskResult, IChildrenTaskInfoFactory childrenTaskInfoFactory);
}
