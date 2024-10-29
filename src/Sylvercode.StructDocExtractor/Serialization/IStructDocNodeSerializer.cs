using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocNodeSerializer
{
    void OnBeforeAsChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream);
    void OnAfterAsChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream);
}

public interface IStructDocNodeSerializer<in TData, in TWriter> : IStructDocNodeSerializer
    where TData : IStructDocNode
    where TWriter : TextWriter
{
    void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream);
    void IStructDocNodeSerializer.OnBeforeAsChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream)
        => OnBeforeAsChildSerialize((TData)node, previousNode, (TWriter)stream);

    void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream);
    void IStructDocNodeSerializer.OnAfterAsChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream)
        => OnAfterAsChildSerialize((TData)node, nextNode, (TWriter)stream);
}
