using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class BaseStructDocNodeHolderSerializer<TData, TChild> : BaseStructDocNodeSerializer<TData>, IStructDocNodeHolderSerializer<TData, TChild>
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    public virtual void OnBetweenChildrenSerialize(TData parent, TChild? previousChild, TChild? nextChild, StreamWriter stream)
    {
        if (previousChild is null)
        {
            if (nextChild is null)
                OnNoChildSerialize(parent, stream);
            else
                OnBeforeFirstChildSerialize(parent, nextChild, stream);
        }
        else
        {
            if (nextChild is null)
                OnAfterLastChildSerialize(parent, previousChild, stream);
            else
                OnBetweenSiblingSerialize(parent, previousChild, nextChild, stream);
        }
    }

    protected virtual void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, StreamWriter stream)
    {
    }

    protected virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, StreamWriter stream)
    {
    }

    protected virtual void OnAfterLastChildSerialize(TData parent, TChild previousChild, StreamWriter stream)
    {
    }

    protected virtual void OnNoChildSerialize(TData parent, StreamWriter stream)
    {
    }
}
