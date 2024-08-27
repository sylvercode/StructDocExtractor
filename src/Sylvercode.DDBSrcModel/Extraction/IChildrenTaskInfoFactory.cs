namespace Sylvercode.DDBSrcModel.Extraction;

public interface IChildrenTaskInfoFactory
{
    IChildrenTaskInfo NewChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData);
}
