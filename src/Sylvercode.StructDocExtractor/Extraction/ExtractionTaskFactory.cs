namespace Sylvercode.StructDocExtractor.Extraction;

public interface IExtractionTaskFactory
{
    IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null);
}

public class ExtractionTaskFactory : IExtractionTaskFactory
{
    public IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null)
        => new ExtractionTask(extractionData, parentTaskInfo);
}
