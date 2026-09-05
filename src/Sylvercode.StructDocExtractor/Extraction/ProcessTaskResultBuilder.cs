using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Fluent builder for constructing <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/> instances step by step.</summary>
/// <typeparam name="TExtractionData">The type of source data element being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used for factory selection.</typeparam>
public class ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator>
{
    private TaskResultType? _resultType;
    private IStructDocNode? _node;
    private TDataDiscriminator? _dataDiscriminator;
    private readonly MetadataDictionary _metadatas = [];
    private IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? _nodeFactoryProvider;
    private readonly List<TExtractionData> _subTasksExtractionData = [];
    private readonly List<TExtractionData> _extraTasksExtractionData = [];

    /// <summary>Constructs and returns the configured <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/>, inferring <see cref="TaskResultType.Skipped"/> when no node or explicit type has been set.</summary>
    /// <returns>The fully configured process task result.</returns>
    public ProcessTaskResult<TExtractionData, TDataDiscriminator> Build()
    {
        TaskResultType resultType = _resultType ??
            (_node is null ? TaskResultType.Skipped : TaskResultType.Success);

        ProcessTaskResult<TExtractionData, TDataDiscriminator> result = new(resultType, _node)
        {
            DataDiscriminator = _dataDiscriminator,
            NodeFactoryProvider = _nodeFactoryProvider,
            SubTasksExtractionData = _subTasksExtractionData,
            ExtraTasksExtractionData = _extraTasksExtractionData,
            Metadatas = _metadatas,
        };

        return result;
    }

    /// <summary>Sets the explicit task result type, overriding the automatic success/skip inference in <see cref="Build"/>.</summary>
    /// <param name="resultType">The result type to assign.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithResultType(TaskResultType resultType)
    {
        _resultType = resultType;
        return this;
    }

    /// <summary>Sets the structural node produced by the factory.</summary>
    /// <param name="node">The produced structural node.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithNode(IStructDocNode node)
    {
        _node = node;
        return this;
    }

    /// <summary>Creates a new node of <typeparamref name="TNode"/> and sets it as the produced node.</summary>
    /// <typeparam name="TNode">The structural node type to instantiate.</typeparam>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithNode<TNode>()
        where TNode : IStructDocNode, new()
        => WithNode(new TNode());

    /// <summary>Sets the discriminator value that identified the matching factory.</summary>
    /// <param name="dataDiscriminator">The discriminator to store in the result.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithDataDiscriminator(
        TDataDiscriminator dataDiscriminator)
    {
        _dataDiscriminator = dataDiscriminator;
        return this;
    }

    /// <summary>Adds a metadata key/value entry to be merged into the extraction result.</summary>
    /// <param name="key">The metadata key name.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithMetadata(string key, object value)
    {
        _metadatas.AddMetadata(key, value);
        return this;
    }

    /// <summary>Sets a scoped factory provider to use for child tasks, overriding the default for the task's subtree.</summary>
    /// <param name="nodeFactoryProvider">The factory provider to scope to this task's children.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithNodeFactoryProvider(
        IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> nodeFactoryProvider)
    {
        _nodeFactoryProvider = nodeFactoryProvider;
        return this;
    }

    /// <summary>Adds a single source item to the sub-task queue, enqueued immediately after the current task.</summary>
    /// <param name="subTaskExtractionData">The source data element to enqueue as a sub-task.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithSubTask(
        TExtractionData subTaskExtractionData)
    {
        _subTasksExtractionData.Add(subTaskExtractionData);
        return this;
    }

    /// <summary>Adds a sequence of source items to the sub-task queue, enqueued immediately after the current task.</summary>
    /// <param name="subTasksExtractionData">The source data elements to enqueue as sub-tasks.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithSubTasks(
        IEnumerable<TExtractionData> subTasksExtractionData)
    {
        _subTasksExtractionData.AddRange(subTasksExtractionData);
        return this;
    }

    /// <summary>Adds a single source item to the extra-task queue, appended at the end of the overall task queue.</summary>
    /// <param name="extraTaskExtractionData">The source data element to enqueue as an extra task.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithExtraTask(
        TExtractionData extraTaskExtractionData)
    {
        _extraTasksExtractionData.Add(extraTaskExtractionData);
        return this;
    }

    /// <summary>Adds a sequence of source items to the extra-task queue, appended at the end of the overall task queue.</summary>
    /// <param name="extraTasksExtractionData">The source data elements to enqueue as extra tasks.</param>
    /// <returns>This builder for chaining.</returns>
    public ProcessTaskResultBuilder<TExtractionData, TDataDiscriminator> WithExtraTasks(
        IEnumerable<TExtractionData> extraTasksExtractionData)
    {
        _extraTasksExtractionData.AddRange(extraTasksExtractionData);
        return this;
    }
}
