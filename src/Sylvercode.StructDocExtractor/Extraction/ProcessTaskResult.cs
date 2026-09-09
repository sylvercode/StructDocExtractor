using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Default implementation of <see cref="IProcessTaskResult{TExtractionData, TDataDiscriminator}"/> returned by node-factory extraction.</summary>
/// <typeparam name="TExtractionData">The type of source data element being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type that identified the matching factory.</typeparam>
/// <param name="resultType">The task outcome classification.</param>
/// <param name="srcNode">The structural node produced by the factory, or <see langword="null"/> when extraction did not produce a node.</param>
public class ProcessTaskResult<TExtractionData, TDataDiscriminator>(TaskResultType resultType, IStructDocNode? srcNode)
    : IProcessTaskResult<TExtractionData, TDataDiscriminator>
{
    /// <inheritdoc/>
    public TaskResultType ResultType => resultType;

    /// <inheritdoc/>
    public IStructDocNode? SrcNode => srcNode;

    /// <summary>Gets or sets the discriminator value that identified the matching factory.</summary>
    public TDataDiscriminator? DataDiscriminator { get; set; }

    /// <summary>Gets or sets the scoped factory provider to use for child tasks, overriding the default for the task's subtree.</summary>
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? NodeFactoryProvider { get; set; }

    /// <summary>Gets or sets the metadata dictionary to merge into the overall extraction result.</summary>
    public MetadataDictionary Metadatas { get; set; } = [];

    /// <summary>Gets or sets the child source items to enqueue as sub-tasks immediately after this task.</summary>
    public List<TExtractionData> SubTasksExtractionData { get; set; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataDiscriminator>.SubTasksExtractionData => SubTasksExtractionData;

    /// <summary>Gets or sets the extra source items to append at the end of the task queue.</summary>
    public List<TExtractionData> ExtraTasksExtractionData { get; set; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataDiscriminator>.ExtraTasksExtractionData => ExtraTasksExtractionData;

    /// <summary>Initializes a successful <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/> carrying the given node.</summary>
    /// <param name="srcNode">The structural node produced by the factory.</param>
    public ProcessTaskResult(IStructDocNode? srcNode = null) : this(TaskResultType.Success, srcNode) { }
}

/// <summary>Static factory helpers for creating error and skipped <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/> instances.</summary>
public static class ProcessTaskResult
{
    /// <summary>Creates a new <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/> with <see cref="TaskResultType.Error"/> and no node.</summary>
    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewError<TExtractionData, TDataDiscriminator>()
        => new(TaskResultType.Error, null);

    /// <summary>Creates a new <see cref="ProcessTaskResult{TExtractionData, TDataDiscriminator}"/> with <see cref="TaskResultType.Skipped"/> and no node.</summary>
    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewSkipped<TExtractionData, TDataDiscriminator>()
        => new(TaskResultType.Skipped, null);

    /// <summary>Creates an error or skipped result depending on <paramref name="asError"/>.</summary>
    /// <param name="asError">When <see langword="true"/>, returns an error result; otherwise a skipped result.</param>
    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewErrorOrSkipped<TExtractionData, TDataDiscriminator>(bool asError)
        => asError ? NewError<TExtractionData, TDataDiscriminator>() : NewSkipped<TExtractionData, TDataDiscriminator>();
}
