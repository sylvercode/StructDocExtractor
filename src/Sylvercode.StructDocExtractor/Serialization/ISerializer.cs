using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface ISerializer
{
    NodeSerializationResult Serialize(IStructDocNode node, TextWriter stream);
}

public interface ISerializer<in TInput> : ISerializer
    where TInput : IStructDocNode
{
    NodeSerializationResult Serialize(TInput obj, TextWriter stream);
    NodeSerializationResult ISerializer.Serialize(IStructDocNode node, TextWriter stream)
        => Serialize((TInput)node, stream);
}

public interface ISerializer<in TInput, in TWriter> : ISerializer<TInput>
    where TInput : IStructDocNode
    where TWriter : TextWriter
{
    NodeSerializationResult Serialize(TInput obj, TWriter stream);
    NodeSerializationResult ISerializer<TInput>.Serialize(TInput node, TextWriter stream)
        => Serialize(node, (TWriter)stream);
}
