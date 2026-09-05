using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Aggregate outcome of an extraction pass, collecting produced structural nodes, metadata, and task statistics.</summary>
public class ExtractionResult
{
    /// <summary>Accumulates task-count statistics for a completed extraction pass.</summary>
    public class ExtractionSummery
    {
        /// <summary>Gets the total number of tasks that were processed during the extraction pass.</summary>
        public int ProcessedTaskCount { get; private set; }
        /// <summary>Gets the number of tasks that completed with an error result.</summary>
        public int ErrorTaskCount { get; private set; }
        /// <summary>Gets the number of tasks that were skipped during the extraction pass.</summary>
        public int SkippedTaskCount { get; private set; }

        /// <summary>Increments the processed count and updates the error or skip counter based on <paramref name="resultType"/>.</summary>
        /// <param name="resultType">The result type of the completed task.</param>
        public void CountTaskResult(TaskResultType resultType)
        {
            ProcessedTaskCount++;
            switch (resultType)
            {
                case TaskResultType.Error:
                    ErrorTaskCount++;
                    break;
                case TaskResultType.Skipped:
                    SkippedTaskCount++;
                    break;
            }
        }
    }

    /// <summary>Initializes a new empty instance of <see cref="ExtractionResult"/>.</summary>
    public ExtractionResult()
    {
    }

    /// <summary>Initializes a new instance of <see cref="ExtractionResult"/> and immediately records the given task result type in the summary.</summary>
    /// <param name="resultType">The result type to count in the initial summary entry.</param>
    public ExtractionResult(TaskResultType resultType)
        => Summery.CountTaskResult(resultType);

    /// <summary>Gets the task-count summary accumulated over the extraction pass.</summary>
    public ExtractionSummery Summery { get; } = new();
    /// <summary>Gets the metadata dictionary aggregated from all processed tasks.</summary>
    public MetadataDictionary Metadatas { get; } = [];
    /// <summary>Gets the list of root structural nodes produced by this extraction pass.</summary>
    public List<IStructDocNode> StructDocNodes { get; } = [];
}
