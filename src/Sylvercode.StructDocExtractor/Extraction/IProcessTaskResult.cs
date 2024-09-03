using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public interface IProcessTaskResult
{
    public TaskResultType ResultType { get; }
    public IStructDocNode? SrcNode { get; }
    public object? DataDiscriminator { get; }
    public object? NodeFactoryProvider { get; }
    public IEnumerable<object> SubTasksExtractionData { get; }
    public IEnumerable<object> ExtraTasksExtractionData { get; }
}

public interface IProcessTaskResult<TExtractionData, TDataDiscriminator> : IProcessTaskResult
{
    new public TDataDiscriminator? DataDiscriminator { get; }
    object? IProcessTaskResult.DataDiscriminator => DataDiscriminator;

    new public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? NodeFactoryProvider { get; }
    object? IProcessTaskResult.NodeFactoryProvider => NodeFactoryProvider;

    new public IEnumerable<TExtractionData> SubTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.SubTasksExtractionData => SubTasksExtractionData.OfType<object>();

    new public IEnumerable<TExtractionData> ExtraTasksExtractionData { get; }
    IEnumerable<object> IProcessTaskResult.ExtraTasksExtractionData => ExtraTasksExtractionData.OfType<object>();
}
