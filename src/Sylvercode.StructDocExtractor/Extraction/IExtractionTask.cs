namespace Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

public interface IExtractionTask
{
    object ExtractionData { get; }
    ParentTaskInfo? ParentTaskInfo { get; }
    IChildrenTaskInfo? ChildrenTaskInfo { get; }
    ExtractionTaskResult? TaskResult { get; }

    event EventHandler? ResultSet;

    void SetResult(IProcessTaskResult processTaskResult, IChildrenTaskInfoFactory childrenTaskInfoFactory);
}
