using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface IExtractionTaskFactory
{
    IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null);
}

public class ExtractionTaskFactory : IExtractionTaskFactory
{
    public IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null)
        => new ExtractionTask(extractionData, parentTaskInfo);
}
