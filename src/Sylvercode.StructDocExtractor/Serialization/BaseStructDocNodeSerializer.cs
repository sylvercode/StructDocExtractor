using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class BaseStructDocNodeSerializer<TData> : IStructDocNodeSerializer<TData>, ISerializer<TData>
    where TData : IStructDocNode
{
    public virtual void Serialize(TData obj, StreamWriter stream)
        => SerializeContent(obj, stream);

    protected virtual void SerializeContent(TData node, StreamWriter stream)
    {
    }

    public virtual void OnBeforeFirstParentChildSerialize(TData node, StreamWriter stream)
    {
    }

    public virtual void OnBeforeNextSiblingSerialize(TData node, TData nextNode, StreamWriter stream)
    {
    }

    public virtual void OnAfterPreviousSiblingSerialize(TData node, TData previousNode, StreamWriter stream)
    {
    }

    public virtual void OnAfterLastParentChildSerialize(TData node, StreamWriter stream)
    {
    }
}
