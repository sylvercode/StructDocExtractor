using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocNodeSerializer
{
    void OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream);
    void OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream);
}

public interface IStructDocNodeSerializer<TData> : IStructDocNodeSerializer
    where TData : IStructDocNode
{
    void OnBeforeChildSerialize(TData node, TData? previousNode, TextWriter stream);
    void IStructDocNodeSerializer.OnBeforeChildSerialize(IStructDocNode node, IStructDocNode? previousNode, TextWriter stream)
        => OnBeforeChildSerialize((TData)node, (TData?)previousNode, stream);

    void OnAfterChildSerialize(TData node, TData? nextNode, TextWriter stream);
    void IStructDocNodeSerializer.OnAfterChildSerialize(IStructDocNode node, IStructDocNode? nextNode, TextWriter stream)
        => OnAfterChildSerialize((TData)node, (TData?)nextNode, stream);
}
