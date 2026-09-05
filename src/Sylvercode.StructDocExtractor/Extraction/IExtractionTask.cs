namespace Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Defines the contract for a single unit of work in the extraction pipeline, carrying source data and tracking its result.</summary>
public interface IExtractionTask
{
    /// <summary>Gets the raw source data to be extracted.</summary>
    object? ExtractionData { get; }
    /// <summary>Gets the parent task information that spawned this task, or <see langword="null"/> for root tasks.</summary>
    ParentTaskInfo? ParentTaskInfo { get; }
    /// <summary>Gets the children task descriptor managing child tasks spawned from this task's result, or <see langword="null"/> if no children have been registered.</summary>
    IChildrenTaskInfo? ChildrenTaskInfo { get; }
    /// <summary>Gets the extraction result once <see cref="SetResult"/> has been called, or <see langword="null"/> if the task is not yet complete.</summary>
    ExtractionTaskResult? TaskResult { get; }

    /// <summary>Raised when the task result is assigned via <see cref="SetResult"/>.</summary>
    event EventHandler? ResultSet;

    /// <summary>Assigns the outcome of processing this task and raises <see cref="ResultSet"/>.</summary>
    /// <param name="processTaskResult">The result produced by the task processor.</param>
    void SetResult(IProcessTaskResult processTaskResult);
}
