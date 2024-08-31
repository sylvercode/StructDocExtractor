using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public interface IProcessTaskResult
{
    public TaskResultType ResultType { get; }
    public ISrcNode? SrcNode { get; }
    public object? DataSelectable { get; }
    public object? NodeFactoryProvider { get; }
    public IEnumerable<object> SubTasksExtractionData { get; }
    public IEnumerable<object> ExtraTasksExtractionData { get; }
}

public interface IProcessTaskResult<TExtractionData, TDataSelectable> : IProcessTaskResult
{
    new public TDataSelectable? DataSelectable { get; }
    object? IProcessTaskResult.DataSelectable => DataSelectable;

    new public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>? NodeFactoryProvider { get; }
    object? IProcessTaskResult.NodeFactoryProvider => NodeFactoryProvider;

    new public IEnumerable<TExtractionData> SubTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.SubTasksExtractionData => SubTasksExtractionData.OfType<object>();

    new public IEnumerable<TExtractionData> ExtraTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.ExtraTasksExtractionData => ExtraTasksExtractionData.OfType<object>();
}
