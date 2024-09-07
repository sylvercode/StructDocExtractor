using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class BaseStructDocNodeHolderSerializer<TData, TChild> : BaseStructDocNodeSerializer<TData>, IStructDocNodeHolderSerializer<TData, TChild>
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    public override void Serialize(TData obj, StreamWriter stream)
        => SerializeContent(obj, stream);

    protected override void SerializeContent(TData node, StreamWriter stream)
    {
    }

    public virtual void OnBeforeFirstChildSerialize(TData parent, TChild child, StreamWriter stream)
    {
    }

    public virtual void OnBetweenSiblingSerialize(TData parent, TChild child1, TChild child2, StreamWriter stream)
    {
    }

    public virtual void OnAfterLastChildSerialize(TData parent, TChild child, StreamWriter stream)
    {
    }
}
