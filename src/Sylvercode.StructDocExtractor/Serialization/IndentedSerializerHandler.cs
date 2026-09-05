using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Serializer handler that injects configurable spacing operations before and after a node is serialized as a child.</summary>
/// <remarks>
/// Plugs into the <see cref="BaseStructDocNodeSerializer{TData,TWriter}.Handler"/> slot to apply
/// <see cref="IndentedStreamWriter.SpaceOperationType"/> transitions (word, line, or paragraph breaks)
/// around each child without requiring subclasses to manage spacing state manually.
/// </remarks>
public class IndentedSerializerHandler<TData, TWriter>(
    IndentedSerializerHandler<TData, TWriter>.Options options)
    : BaseStructDocNodeSerializer<TData, TWriter>.Handler
        where TData : IStructDocNode
        where TWriter : IndentedStreamWriter
{
    /// <summary>Configuration options for <see cref="IndentedSerializerHandler{TData,TWriter}"/>.</summary>
    public class Options(IndentedStreamWriter.SpaceOperationType spaceBeforeAfterChild = IndentedStreamWriter.SpaceOperationType.None)
    {
        /// <summary>Gets or sets a value indicating whether spacing operations are skipped for root-level nodes.</summary>
        public bool SpaceOperationOnlyOnNoneRoot { get; set; } = true;

        /// <summary>Gets or sets the spacing operation applied before a child node is serialized.</summary>
        public IndentedStreamWriter.SpaceOperationType SpaceBeforeChild { get; set; } = spaceBeforeAfterChild;

        /// <summary>Gets or sets the spacing operation ensured after a child node is serialized.</summary>
        public IndentedStreamWriter.SpaceOperationType SpaceAfterChild { get; set; } = spaceBeforeAfterChild;

        /// <summary>Returns <see langword="true"/> when a spacing operation should be applied to <paramref name="node"/>.</summary>
        /// <param name="node">The node being evaluated.</param>
        /// <returns><see langword="true"/> unless <see cref="SpaceOperationOnlyOnNoneRoot"/> is set and the node is a root.</returns>
        public bool MustDoSpaceOperation(TData node)
            => !SpaceOperationOnlyOnNoneRoot || !node.IsRoot;
    }

    private readonly Options _options = options;

    /// <inheritdoc/>
    public override void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
    {
        if (_options.MustDoSpaceOperation(node))
            stream.DoSpaceOperation(_options.SpaceBeforeChild);
    }

    /// <inheritdoc/>
    public override void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
    {
        if (_options.MustDoSpaceOperation(node))
            stream.EnsureSpaceOperation(_options.SpaceAfterChild);
    }
}
