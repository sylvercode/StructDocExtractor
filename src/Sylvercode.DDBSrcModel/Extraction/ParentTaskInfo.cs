namespace Sylvercode.DDBSrcModel.Extraction;

public class ParentTaskInfo(ExtractionTask parentTask, int[] siblingSubTaskIndex)
{
    public ExtractionTask ParentTask => parentTask;
    public int[] SiblingSubTaskIndex => siblingSubTaskIndex;
}
