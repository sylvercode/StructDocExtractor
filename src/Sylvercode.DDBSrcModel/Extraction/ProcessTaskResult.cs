using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ProcessTaskResult<TExtractionData, TDataSelectable>(TaskResultType resultType, ISrcNode? srcNode) : IProcessTaskResult<TExtractionData, TDataSelectable>
{
    public static ProcessTaskResult<TExtractionData, TDataSelectable> Error { get; } = new(TaskResultType.Error, null);

    public static ProcessTaskResult<TExtractionData, TDataSelectable> Skipped { get; } = new(TaskResultType.Skipped, null);

    public TaskResultType ResultType => resultType;

    public ISrcNode? SrcNode => srcNode;

    public TDataSelectable? DataSelectable { get; set; }

    public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>? NodeFactoryProvider { get; set; }

    public List<TExtractionData> SubTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.SubTasksExtractionData => SubTasksExtractionData;

    public List<TExtractionData> ExtraTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.ExtraTasksExtractionData => ExtraTasksExtractionData;

    public ProcessTaskResult(ISrcNode? srcNode = null) : this(TaskResultType.Success, srcNode) { }
}
