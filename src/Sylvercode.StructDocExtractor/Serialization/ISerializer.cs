using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface ISerializer
{
    void Serialize(IStructDocNode node, StreamWriter stream);
}

public interface ISerializer<in TInput> : ISerializer
    where TInput : IStructDocNode
{
    void Serialize(TInput obj, StreamWriter stream);
    void ISerializer.Serialize(IStructDocNode node, StreamWriter stream)
        => Serialize((TInput)node, stream);
}
