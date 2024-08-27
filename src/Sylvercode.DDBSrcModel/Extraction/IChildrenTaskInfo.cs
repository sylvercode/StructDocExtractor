namespace Sylvercode.DDBSrcModel.Extraction;

public interface IChildrenTaskInfo
{
    ExtractionTask Task { get; }

    IReadOnlyList<ExtractionTask> ChildrenTasks { get; }

    IReadOnlyList<TaskIndex> PendingSubTaskIndex { get; }

    bool HasPendingSubTaskIndex { get; }

    void RegisterGrandChildTaskResultSet(ExtractionTask task, TaskIndex taskIndex);
}
