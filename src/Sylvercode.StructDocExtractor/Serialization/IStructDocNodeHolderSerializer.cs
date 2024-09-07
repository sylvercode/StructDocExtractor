using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


public interface IStructDocNodeHolderSerializer : IStructDocNodeSerializer
{
    void OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, StreamWriter stream);
}

public interface IStructDocNodeHolderSerializer<TData, TChild> : IStructDocNodeSerializer<TData>, IStructDocNodeHolderSerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    void OnBetweenChildrenSerialize(TData parent, TChild? previousChild, TChild? nextChild, StreamWriter stream);
    void IStructDocNodeHolderSerializer.OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, StreamWriter stream)
        => OnBetweenChildrenSerialize((TData)parent, (TChild?)previousChild, (TChild?)nextChild, stream);
}
