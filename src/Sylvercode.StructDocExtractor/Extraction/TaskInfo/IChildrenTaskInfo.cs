namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Defines the contract for tracking the child extraction tasks spawned from a parent extraction result, including pending and completed sub-task indices.</summary>
public interface IChildrenTaskInfo
{
    /// <summary>Gets the parent extraction task that spawned these child tasks.</summary>
    ExtractionTask Task { get; }

    /// <summary>Gets the ordered list of all child extraction tasks created from the parent result.</summary>
    IReadOnlyList<ExtractionTask> ChildrenTasks { get; }

    /// <summary>Gets the set of task indices that have not yet had their results set.</summary>
    IReadOnlyList<TaskIndex> PendingSubTaskIndex { get; }

    /// <summary>Gets a value indicating whether any child tasks are still pending completion.</summary>
    bool HasPendingSubTaskIndex { get; }

    /// <summary>Registers <paramref name="taskIndex"/> as a pending child task awaiting its result.</summary>
    /// <param name="taskIndex">The index of the child task to mark as pending.</param>
    void AddPendingSubTaskIndex(TaskIndex taskIndex);

    /// <summary>Signals that the child task at <paramref name="taskIndex"/> has produced its result, removing it from the pending set.</summary>
    /// <param name="taskIndex">The index of the completed child task.</param>
    /// <param name="subTask">The completed child task.</param>
    void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask);
}
