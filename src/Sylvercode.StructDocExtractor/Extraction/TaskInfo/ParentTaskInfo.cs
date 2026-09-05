namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Carries the parent task reference and sibling index for a child extraction task, providing upward navigation within the extraction task tree.</summary>
/// <param name="parentTask">The parent extraction task that spawned this child.</param>
/// <param name="siblingSubTaskIndex">The positional index of this task among its siblings.</param>
public class ParentTaskInfo(ExtractionTask parentTask, TaskIndex siblingSubTaskIndex)
{
    /// <summary>Gets the parent extraction task that spawned this child task.</summary>
    public ExtractionTask ParentTask => parentTask;
    /// <summary>Gets the positional index of this task within its parent's child sequence.</summary>
    public TaskIndex SiblingSubTaskIndex => siblingSubTaskIndex;
}
