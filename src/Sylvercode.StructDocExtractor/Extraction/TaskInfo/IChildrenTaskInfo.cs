namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

public interface IChildrenTaskInfo
{
    ExtractionTask Task { get; }

    IReadOnlyList<ExtractionTask> ChildrenTasks { get; }

    IReadOnlyList<TaskIndex> PendingSubTaskIndex { get; }

    bool HasPendingSubTaskIndex { get; }

    void AddPendingSubTaskIndex(TaskIndex taskIndex);

    void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask);
}
