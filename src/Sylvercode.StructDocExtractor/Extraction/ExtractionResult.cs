using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ExtractionResult
{
    public class ExtractionSummery
    {
        public int ProcessedTaskCount { get; private set; }
        public int ErrorTaskCount { get; private set; }
        public int SkippedTaskCount { get; private set; }

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

    public ExtractionResult()
    {
    }

    public ExtractionResult(TaskResultType resultType)
        => Summery.CountTaskResult(resultType);

    public ExtractionSummery Summery { get; } = new();
    public MetadataDictionary Metadatas { get; } = [];
    public List<IStructDocNode> StructDocNodes { get; } = [];
}
