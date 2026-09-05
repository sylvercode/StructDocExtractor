using Microsoft.Extensions.Logging;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Specialization of <see cref="ChildrenTaskInfo"/> that propagates pending-task events up to an ancestor's <see cref="IChildrenTaskInfo"/>, maintaining a unified view of pending work across multiple hierarchy levels.</summary>
/// <remarks>
/// Each <see cref="AddPendingSubTaskIndex"/> and <see cref="OnSubTaskResultSet"/> call is forwarded to the
/// parent task's <see cref="IChildrenTaskInfo"/> using a composite <see cref="TaskIndex"/> that combines
/// the current task's sibling index with the local child index. This ensures the grandparent tracking layer
/// remains aware of all descendants' progress without duplicating state management logic.
/// </remarks>
public class ProxyChildrenTaskInfo(ExtractionTask task,
                                   IEnumerable<object>? childrenData,
                                   ILogger<ProxyChildrenTaskInfo>? logger = null)
    : ChildrenTaskInfo(task, childrenData, untypedLogger: logger)
{
    private readonly IChildrenTaskInfo _parentChildrenTaskInfo
        = task.ParentTaskInfo?.ParentTask?.ChildrenTaskInfo
        ?? throw new ArgumentException($"{nameof(task)} has no parent task info with children info.");

    /// <inheritdoc/>
    public override void AddPendingSubTaskIndex(TaskIndex taskIndex)
    {
        base.AddPendingSubTaskIndex(taskIndex);
        _parentChildrenTaskInfo.AddPendingSubTaskIndex(GetIndexForParent(taskIndex));
    }

    /// <summary>Returns a composite <see cref="TaskIndex"/> combining this task's sibling index with <paramref name="nextTaskIndex"/> for use in the parent tracking layer.</summary>
    /// <param name="nextTaskIndex">The local child task index to promote to the parent scope.</param>
    /// <returns>A parent-relative <see cref="TaskIndex"/>.</returns>
    private TaskIndex GetIndexForParent(TaskIndex nextTaskIndex)
        => new(Task.ParentTaskInfo!.SiblingSubTaskIndex, nextTaskIndex);

    /// <inheritdoc/>
    public override void OnSubTaskResultSet(TaskIndex taskIndex, ExtractionTask subTask)
    {
        base.OnSubTaskResultSet(taskIndex, subTask);
        _parentChildrenTaskInfo.OnSubTaskResultSet(GetIndexForParent(taskIndex), subTask);
    }
}
