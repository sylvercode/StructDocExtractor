using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ProcessTaskResult<TExtractionData, TDataSelectable>(ISrcNode? srcNode) : IProcessTaskResult<TExtractionData, TDataSelectable>
{
    public ISrcNode? SrcNode => srcNode;

    public TDataSelectable? DataSelectable { get; set; }

    public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>? NodeFactoryProvider { get; set; }

    public List<TExtractionData> SubTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.SubTasksExtractionData => SubTasksExtractionData;

    public List<TExtractionData> ExtraTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.ExtraTasksExtractionData => ExtraTasksExtractionData;
}
