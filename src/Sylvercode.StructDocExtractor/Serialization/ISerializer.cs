using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Base contract for all node serializers in the serialization pipeline.</summary>
public interface ISerializer
{
    /// <summary>Serializes <paramref name="node"/> to the provided <paramref name="stream"/>.</summary>
    /// <param name="node">The structural document node to serialize.</param>
    /// <param name="stream">The writer to which serialized output is written.</param>
    /// <returns>A <see cref="NodeSerializationResult"/> indicating whether content was directly emitted.</returns>
    NodeSerializationResult Serialize(IStructDocNode node, TextWriter stream);
}

/// <summary>Typed serializer contract for a specific <typeparamref name="TInput"/> node type.</summary>
/// <typeparam name="TInput">The concrete <see cref="IStructDocNode"/> type this serializer handles.</typeparam>
public interface ISerializer<in TInput> : ISerializer
    where TInput : IStructDocNode
{
    /// <summary>Serializes the typed <paramref name="obj"/> to the provided <paramref name="stream"/>.</summary>
    /// <param name="obj">The typed structural document node to serialize.</param>
    /// <param name="stream">The writer to which serialized output is written.</param>
    /// <returns>A <see cref="NodeSerializationResult"/> indicating whether content was directly emitted.</returns>
    NodeSerializationResult Serialize(TInput obj, TextWriter stream);
    NodeSerializationResult ISerializer.Serialize(IStructDocNode node, TextWriter stream)
        => Serialize((TInput)node, stream);
}

/// <summary>Typed serializer contract for a specific node type and a specific writer type.</summary>
/// <typeparam name="TInput">The concrete <see cref="IStructDocNode"/> type this serializer handles.</typeparam>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype used for output.</typeparam>
public interface ISerializer<in TInput, in TWriter> : ISerializer<TInput>
    where TInput : IStructDocNode
    where TWriter : TextWriter
{
    /// <summary>Serializes <paramref name="obj"/> to the typed <paramref name="stream"/>.</summary>
    /// <param name="obj">The typed structural document node to serialize.</param>
    /// <param name="stream">The typed writer to which serialized output is written.</param>
    /// <returns>A <see cref="NodeSerializationResult"/> indicating whether content was directly emitted.</returns>
    NodeSerializationResult Serialize(TInput obj, TWriter stream);
    NodeSerializationResult ISerializer<TInput>.Serialize(TInput node, TextWriter stream)
        => Serialize(node, (TWriter)stream);
}
