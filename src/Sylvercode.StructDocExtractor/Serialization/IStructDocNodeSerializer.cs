using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Contract for serializer callbacks invoked before and after a node is serialized as a child of its parent.</summary>
public interface IStructDocNodeSerializer
{
    /// <summary>Invoked immediately before <paramref name="node"/> is serialized as a child of its parent.</summary>
    /// <param name="node">The node about to be serialized.</param>
    /// <param name="previousNode">The previously serialized sibling, or <see langword="null"/> if this is the first child.</param>
    /// <param name="stream">The writer receiving serialized output.</param>
    void OnBeforeAsChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream);

    /// <summary>Invoked immediately after <paramref name="node"/> has been serialized as a child of its parent.</summary>
    /// <param name="node">The node that was just serialized.</param>
    /// <param name="nextNode">The next sibling to be serialized, or <see langword="null"/> if this is the last child.</param>
    /// <param name="stream">The writer receiving serialized output.</param>
    void OnAfterAsChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream);
}

/// <summary>Typed <see cref="IStructDocNodeSerializer"/> callback contract for a specific node and writer type.</summary>
/// <typeparam name="TData">The concrete <see cref="IStructDocNode"/> type these callbacks handle.</typeparam>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype used during serialization.</typeparam>
public interface IStructDocNodeSerializer<in TData, in TWriter> : IStructDocNodeSerializer
    where TData : IStructDocNode
    where TWriter : TextWriter
{
    /// <inheritdoc cref="IStructDocNodeSerializer.OnBeforeAsChildSerialize"/>
    void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream);
    void IStructDocNodeSerializer.OnBeforeAsChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream)
        => OnBeforeAsChildSerialize((TData)node, previousNode, (TWriter)stream);

    /// <inheritdoc cref="IStructDocNodeSerializer.OnAfterAsChildSerialize"/>
    void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream);
    void IStructDocNodeSerializer.OnAfterAsChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream)
        => OnAfterAsChildSerialize((TData)node, nextNode, (TWriter)stream);
}
