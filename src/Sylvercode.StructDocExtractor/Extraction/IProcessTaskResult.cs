using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the untyped contract for the outcome returned by an extraction task processor, including the produced node, discriminator, factory provider, and child task data.</summary>
public interface IProcessTaskResult
{
    /// <summary>Gets the task result classification indicating success, warning, error, or skip.</summary>
    public TaskResultType ResultType { get; }
    /// <summary>Gets the structural node produced by this task, or <see langword="null"/> if no node was created.</summary>
    public IStructDocNode? SrcNode { get; }
    /// <summary>Gets the untyped data discriminator that was used to select the factory, or <see langword="null"/>.</summary>
    public object? DataDiscriminator { get; }
    /// <summary>Gets the untyped factory provider selected for this task, or <see langword="null"/>.</summary>
    public object? NodeFactoryProvider { get; }
    /// <summary>Gets the metadata dictionary populated during extraction.</summary>
    public MetadataDictionary Metadatas { get; }
    /// <summary>Gets the sequence of source data items to be dispatched as child extraction tasks.</summary>
    public IEnumerable<object> SubTasksExtractionData { get; }
    /// <summary>Gets additional source data items to be dispatched as sibling extraction tasks outside the normal child hierarchy.</summary>
    public IEnumerable<object> ExtraTasksExtractionData { get; }
}

/// <summary>Typed extension of <see cref="IProcessTaskResult"/> that exposes strongly-typed discriminator, factory provider, and child data collections.</summary>
/// <typeparam name="TExtractionData">The type of source data being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of the data discriminator used to select the node factory.</typeparam>
public interface IProcessTaskResult<TExtractionData, TDataDiscriminator> : IProcessTaskResult
{
    /// <summary>Gets the typed data discriminator selected for this extraction task.</summary>
    new public TDataDiscriminator? DataDiscriminator { get; }
    /// <inheritdoc/>
    object? IProcessTaskResult.DataDiscriminator => DataDiscriminator;

    /// <summary>Gets the typed factory provider resolved for this task.</summary>
    new public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? NodeFactoryProvider { get; }
    /// <inheritdoc/>
    object? IProcessTaskResult.NodeFactoryProvider => NodeFactoryProvider;

    /// <summary>Gets the typed collection of child source data items to extract.</summary>
    new public IEnumerable<TExtractionData> SubTasksExtractionData { get; }
    /// <inheritdoc/>
    IEnumerable<object> IProcessTaskResult.SubTasksExtractionData => SubTasksExtractionData.OfType<object>();

    /// <summary>Gets the typed collection of extra sibling source data items to extract.</summary>
    new public IEnumerable<TExtractionData> ExtraTasksExtractionData { get; }
    /// <inheritdoc/>
    IEnumerable<object> IProcessTaskResult.ExtraTasksExtractionData => ExtraTasksExtractionData.OfType<object>();
}
