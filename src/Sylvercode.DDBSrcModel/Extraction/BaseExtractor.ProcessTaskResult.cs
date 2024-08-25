using Sylvercode.DDBSrcModel.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<ExtractionData, DataSelectable> where ExtractionData : notnull
{
    public class ProcessTaskResult(ISrcNode srcNode) : IProcessTaskResult
    {
        public ISrcNode SrcNode => srcNode;
        public DataSelectable? DataSelectable { get; set; }
        public ISrcNodeFactoryProvider<DataSelectable>? NodeFactoryProvider { get; set; }
        public List<ExtractionData> SubTasksExtractionData { get; } = [];
        public List<ExtractionData> OtherTasksExtractionData { get; } = [];

        object? IProcessTaskResult.DataSelectable => DataSelectable;
        object? IProcessTaskResult.NodeFactoryProvider => NodeFactoryProvider;
        IEnumerable<object> IProcessTaskResult.SubTasksExtractionData => SubTasksExtractionData.Cast<object>();
    }
}
