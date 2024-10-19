using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


public interface IStructDocNodeHolderSerializer : IStructDocNodeSerializer
{
    void OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, TextWriter stream);
}

public interface IStructDocNodeHolderSerializer<in TData, in TWriter, TChild> : IStructDocNodeSerializer<TData, TWriter>, IStructDocNodeHolderSerializer
    where TData : IStructDocNodeHolder<TChild>
    where TWriter : TextWriter
    where TChild : class, IStructDocNode
{
    void OnBetweenChildrenSerialize(TData parent, TChild? previousChild, TChild? nextChild, TWriter stream);
    void IStructDocNodeHolderSerializer.OnBetweenChildrenSerialize(IStructDocNodeHolder parent, IStructDocNode? previousChild, IStructDocNode? nextChild, TextWriter stream)
        => OnBetweenChildrenSerialize((TData)parent, (TChild?)previousChild, (TChild?)nextChild, (TWriter)stream);
}
