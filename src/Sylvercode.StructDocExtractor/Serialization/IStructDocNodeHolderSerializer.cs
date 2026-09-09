using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


/// <summary>Extends <see cref="IStructDocNodeSerializer"/> with a callback invoked between a holder node's serialized children.</summary>
public interface IStructDocNodeHolderSerializer : IStructDocNodeSerializer
{
    /// <summary>Invoked between consecutive children (and before the first and after the last) during holder serialization.</summary>
    /// <param name="parent">The holder node whose children are being serialized.</param>
    /// <param name="previousChild">The child just serialized, or <see langword="null"/> before the first child.</param>
    /// <param name="nextChild">The child about to be serialized, or <see langword="null"/> after the last child.</param>
    /// <param name="stream">The writer receiving serialized output.</param>
    void OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, TextWriter stream);
}

/// <summary>Typed holder-serializer callback contract for a specific holder, writer, and child type.</summary>
/// <typeparam name="TData">The concrete holder node type.</typeparam>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype used during serialization.</typeparam>
/// <typeparam name="TChild">The type of child nodes held by <typeparamref name="TData"/>.</typeparam>
public interface IStructDocNodeHolderSerializer<in TData, in TWriter, TChild> : IStructDocNodeSerializer<TData, TWriter>, IStructDocNodeHolderSerializer
    where TData : IStructDocNodeHolder<TChild>
    where TWriter : TextWriter
    where TChild : class, IStructDocNode
{
    /// <inheritdoc cref="IStructDocNodeHolderSerializer.OnBetweenChildrenSerialize"/>
    void OnBetweenChildrenSerialize(TData parent, TChild? previousChild, TChild? nextChild, TWriter stream);
    void IStructDocNodeHolderSerializer.OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, TextWriter stream)
        => OnBetweenChildrenSerialize((TData)parent, (TChild?)previousChild, (TChild?)nextChild, (TWriter)stream);
}
