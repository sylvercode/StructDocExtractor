namespace Sylvercode.StructDocExtractor.Extraction;

public class ParentTaskInfo(ExtractionTask parentTask, TaskIndex siblingSubTaskIndex)
{
    public ExtractionTask ParentTask => parentTask;
    public TaskIndex SiblingSubTaskIndex => siblingSubTaskIndex;
}
