using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface ISerializer
{
    void Serialize(IStructDocNode node, TextWriter stream);
}

public interface ISerializer<in TInput> : ISerializer
    where TInput : IStructDocNode
{
    void Serialize(TInput obj, TextWriter stream);
    void ISerializer.Serialize(IStructDocNode node, TextWriter stream)
        => Serialize((TInput)node, stream);
}
