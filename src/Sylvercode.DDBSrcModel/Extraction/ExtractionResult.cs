using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionResult(ExtractionResult.ExtractionSummery summery,
                              IReadOnlyList<ISrcNode> srcNodes)
{
    public class ExtractionSummery
    {
        public int ProcessedTaskCount { get; private set; } = 0;
        public int ErrorTaskCount { get; private set; } = 0;
        public int SkippedTaskCount { get; private set; } = 0;

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
    public IReadOnlyList<ISrcNode> SrcNodes { get; } = srcNodes;
}
