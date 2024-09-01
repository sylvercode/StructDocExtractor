namespace Sylvercode.DDBSrcModel.Extraction;

public interface IExtractionTask
{
    object ExtractionData { get; }
    ParentTaskInfo? ParentTaskInfo { get; }
    IChildrenTaskInfo? ChildrenTaskInfo { get; }
    ExtractionTaskResult? TaskResult { get; }

    public delegate void ResultSetEventHandler(ExtractionTask sender);
    event ResultSetEventHandler? ResultSet;

    void SetResult(IProcessTaskResult processTaskResult, IChildrenTaskInfoFactory childrenTaskInfoFactory);
}
