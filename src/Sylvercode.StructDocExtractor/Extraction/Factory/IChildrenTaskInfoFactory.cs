using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface IChildrenTaskInfoFactory
{
    IChildrenTaskInfo NewChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData);
}
