using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocNodeSerializer
{
    void OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, StreamWriter stream);
    void OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, StreamWriter stream);
}

public interface IStructDocNodeSerializer<TData> : IStructDocNodeSerializer
    where TData : IStructDocNode
{
    void OnBeforeChildSerialize(TData node, TData? previousNode, StreamWriter stream);
    void IStructDocNodeSerializer.OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, StreamWriter stream)
        => OnBeforeChildSerialize((TData)node, (TData?)previousNode, stream);

    void OnAfterChildSerialize(TData node, TData? nextNode, StreamWriter stream);
    void IStructDocNodeSerializer.OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, StreamWriter stream)
        => OnAfterChildSerialize((TData)node, (TData?)nextNode, stream);
}
