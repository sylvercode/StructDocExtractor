using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Contract for creating <see cref="IExtractionTask"/> instances from raw source data.</summary>
public interface IExtractionTaskFactory
{
    /// <summary>Creates and returns a new extraction task for the given source data and optional parent context.</summary>
    /// <param name="extractionData">The source data element to extract a node from.</param>
    /// <param name="parentTaskInfo">Optional parent-context descriptor; <see langword="null"/> for root tasks.</param>
    /// <returns>A new <see cref="IExtractionTask"/> ready to be enqueued.</returns>
    IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null);
}

/// <summary>Default implementation of <see cref="IExtractionTaskFactory"/> that creates <see cref="ExtractionTask"/> instances.</summary>
public class ExtractionTaskFactory : IExtractionTaskFactory
{
    /// <inheritdoc/>
    public IExtractionTask NewTask(object extractionData, ParentTaskInfo? parentTaskInfo = null)
        => new ExtractionTask(extractionData, parentTaskInfo);
}
