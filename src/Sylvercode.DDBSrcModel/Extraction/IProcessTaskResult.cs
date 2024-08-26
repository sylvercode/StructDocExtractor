using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public interface IProcessTaskResult
{
    public ISrcNode SrcNode { get; }
    public object? DataSelectable { get; }
    public object? NodeFactoryProvider { get; }
    public IEnumerable<object> SubTasksExtractionData { get; }
    public IEnumerable<object> ExtraTasksExtractionData { get; }
}

public interface IProcessTaskResult<out TExtractionData> : IProcessTaskResult
{
    new public IEnumerable<TExtractionData> SubTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.SubTasksExtractionData => SubTasksExtractionData.OfType<object>();
    new public IEnumerable<TExtractionData> ExtraTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.ExtraTasksExtractionData => ExtraTasksExtractionData.OfType<object>();
}

public interface IProcessTaskResult<TExtractionData, TDataSelectable> : IProcessTaskResult<TExtractionData>
{
    new public TDataSelectable? DataSelectable { get; }
    object? IProcessTaskResult.DataSelectable => DataSelectable;
    new public ISrcNodeFactoryProvider<TDataSelectable>? NodeFactoryProvider { get; }
    object? IProcessTaskResult.NodeFactoryProvider => NodeFactoryProvider;
}
