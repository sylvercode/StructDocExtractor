using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public interface IProcessTaskResult
{
    public ISrcNode SrcNode { get; }
    public object? DataSelectable { get; }
    public object? NodeFactoryProvider { get; }
    public IEnumerable<object> SubTasksExtractionData { get; }
}
