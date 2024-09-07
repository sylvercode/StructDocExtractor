using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class BaseStructDocNodeSerializer<TData> : IStructDocNodeSerializer<TData>, ISerializer<TData>
    where TData : IStructDocNode
{
    public virtual void Serialize(TData obj, StreamWriter stream)
    {
    }

    public virtual void OnBeforeChildSerialize(TData node, TData? previousNode, StreamWriter stream)
    {
    }

    public virtual void OnAfterChildSerialize(TData node, TData? nextNode, StreamWriter stream)
    {
    }
}
