namespace Sylvercode.DDBSrcModel.Extraction;

public class ParentTaskInfo(ExtractionTask parentTask, int siblingSubTaskIndex)
{
    public ExtractionTask ParnetTask => parentTask;
    public int SiblingSubTaskIndex => siblingSubTaskIndex;
}
