namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

public class ParentTaskInfo(ExtractionTask parentTask, TaskIndex siblingSubTaskIndex)
{
    public ExtractionTask ParentTask => parentTask;
    public TaskIndex SiblingSubTaskIndex => siblingSubTaskIndex;
}
