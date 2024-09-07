using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocNodeSerializer
{
    void OnBeforeFirstParentChildSerialize(IStructDocNode node, StreamWriter stream);
    void OnBeforeNextSiblingSerialize(IStructDocNode node, IStructDocNode nextNode, StreamWriter stream);
    void OnAfterPreviousSiblingSerialize(IStructDocNode node, IStructDocNode previousNode, StreamWriter stream);
    void OnAfterLastParentChildSerialize(IStructDocNode node, StreamWriter stream);
}

public interface IStructDocNodeSerializer<TData> : IStructDocNodeSerializer
    where TData : IStructDocNode
{
    void OnBeforeFirstParentChildSerialize(TData node, StreamWriter stream);
    void IStructDocNodeSerializer.OnBeforeFirstParentChildSerialize(IStructDocNode node, StreamWriter stream)
        => OnBeforeFirstParentChildSerialize((TData)node, stream);

    void OnBeforeNextSiblingSerialize(TData node, TData nextNode, StreamWriter stream);
    void IStructDocNodeSerializer.OnBeforeNextSiblingSerialize(IStructDocNode node, IStructDocNode nextNode, StreamWriter stream)
        => OnBeforeNextSiblingSerialize((TData)node, (TData)nextNode, stream);

    void OnAfterPreviousSiblingSerialize(TData node, TData previousNode, StreamWriter stream);
    void IStructDocNodeSerializer.OnAfterPreviousSiblingSerialize(IStructDocNode node, IStructDocNode previousNode, StreamWriter stream)
        => OnAfterPreviousSiblingSerialize((TData)node, (TData)previousNode, stream);

    void OnAfterLastParentChildSerialize(TData node, StreamWriter stream);
    void IStructDocNodeSerializer.OnAfterLastParentChildSerialize(IStructDocNode node, StreamWriter stream)
        => OnAfterLastParentChildSerialize((TData)node, stream);
}
