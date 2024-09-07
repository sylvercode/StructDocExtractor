using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


public interface IStructDocNodeHolderSerializer : IStructDocNodeSerializer
{
    void OnBeforeFirstChildSerialize(IStructDocNodeHolder parent, IStructDocNode child, StreamWriter stream);
    void OnBetweenSiblingSerialize(IStructDocNodeHolder parent, IStructDocNode child1, IStructDocNode child2, StreamWriter stream);
    void OnAfterLastChildSerialize(IStructDocNodeHolder parent, IStructDocNode child, StreamWriter stream);
}

public interface IStructDocNodeHolderSerializer<TData, TChild> : IStructDocNodeSerializer<TData>, IStructDocNodeHolderSerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    void OnBeforeFirstChildSerialize(TData parent, TChild child, StreamWriter stream);
    void IStructDocNodeHolderSerializer.OnBeforeFirstChildSerialize(IStructDocNodeHolder parent, IStructDocNode child, StreamWriter stream)
        => OnBeforeFirstChildSerialize((TData)parent, (TChild)child, stream);

    void OnBetweenSiblingSerialize(TData parent, TChild IStructDocNode, TChild child2, StreamWriter stream);
    void IStructDocNodeHolderSerializer.OnBetweenSiblingSerialize(IStructDocNodeHolder parent, IStructDocNode child1, IStructDocNode child2, StreamWriter stream)
        => OnBetweenSiblingSerialize((TData)parent, (TChild)child1, (TChild)child2, stream);

    void OnAfterLastChildSerialize(TData parent, TChild child, StreamWriter stream);
    void IStructDocNodeHolderSerializer.OnAfterLastChildSerialize(IStructDocNodeHolder parent, IStructDocNode child, StreamWriter stream)
        => OnAfterLastChildSerialize((TData)parent, (TChild)child, stream);
}
