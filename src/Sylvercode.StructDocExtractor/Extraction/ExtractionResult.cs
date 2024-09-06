using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ExtractionResult(ExtractionResult.ExtractionSummery summery,
                              IReadOnlyList<IStructDocNode> srcNodes)
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

    public ExtractionSummery Summery { get; } = summery;
    public IReadOnlyList<IStructDocNode> StructDocNodes { get; } = srcNodes;
}
