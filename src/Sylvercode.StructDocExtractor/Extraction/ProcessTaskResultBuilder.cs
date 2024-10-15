using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator>
{
    private TaskResultType? _resultType;
    private IStructDocNode? _node;
    private TDataDiscriminator? _dataDiscriminator;
    private readonly MetadataDictionary _metadatas = [];
    private IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? _nodeFactoryProvider;
    private readonly List<TExtractionData> _subTasksExtractionData = [];
    private readonly List<TExtractionData> _extraTasksExtractionData = [];

    public ProcessTaskResult<TExtractionData, TDataDiscriminator> Build()
    {
        TaskResultType resultType = _resultType ??
            (_node is null ? TaskResultType.Skipped : TaskResultType.Success);

        ProcessTaskResult<TExtractionData, TDataDiscriminator> result = new(resultType, _node)
        {
            DataDiscriminator = _dataDiscriminator,
            NodeFactoryProvider = _nodeFactoryProvider,
            SubTasksExtractionData = _subTasksExtractionData,
            ExtraTasksExtractionData = _extraTasksExtractionData
        };

        return result;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithResultType(TaskResultType resultType)
    {
        _resultType = resultType;
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithNode(IStructDocNode node)
    {
        _node = node;
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithDataDiscriminator(
        TDataDiscriminator dataDiscriminator)
    {
        _dataDiscriminator = dataDiscriminator;
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithMetadata(string key, object value)
    {
        _metadatas.AddMetadata(key, value);
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithNodeFactoryProvider(
        IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> nodeFactoryProvider)
    {
        _nodeFactoryProvider = nodeFactoryProvider;
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithSubTasksExtractionData(
        TExtractionData subTaskExtractionData)
    {
        _subTasksExtractionData.Add(subTaskExtractionData);
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithSubTasksExtractionData(
        IEnumerable<TExtractionData> subTasksExtractionData)
    {
        _subTasksExtractionData.AddRange(subTasksExtractionData);
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithExtraTasksExtractionData(
        TExtractionData extraTaskExtractionData)
    {
        _extraTasksExtractionData.Add(extraTaskExtractionData);
        return this;
    }

    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithExtraTasksExtractionData(
        IEnumerable<TExtractionData> extraTasksExtractionData)
    {
        _extraTasksExtractionData.AddRange(extraTasksExtractionData);
        return this;
    }
}
