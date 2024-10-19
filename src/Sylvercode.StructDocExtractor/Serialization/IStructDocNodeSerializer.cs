using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocNodeSerializer
{
    void OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream);
    void OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream);
}

public interface IStructDocNodeSerializer<in TData, in TWriter> : IStructDocNodeSerializer
    where TData : IStructDocNode
    where TWriter : TextWriter
{
    void OnBeforeChildSerialize(TData node, TData? previousNode, TWriter stream);
    void IStructDocNodeSerializer.OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream)
        => OnBeforeChildSerialize((TData)node, (TData?)previousNode, (TWriter)stream);

    void OnAfterChildSerialize(TData node, TData? nextNode, TWriter stream);
    void IStructDocNodeSerializer.OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream)
        => OnAfterChildSerialize((TData)node, (TData?)nextNode, (TWriter)stream);
}
