using System.Diagnostics;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcNode<P>(string id) :
    ISrcNode<P>, ISrcNodeIntializer<P>
    where P : class, ISrcNodeHolder
{
    private P? _parent;
    virtual public P Parent
    {
        get => _parent ?? throw new InvalidOperationException($"{nameof(Parent)} has not yet been initilized.");
    }

    public virtual bool IsRoot => false;
    public virtual bool IsLeaf => true;

    public string Id { get; } = id;

    public string NodeSnippet() => ((ISrcNode)this).DebugName;

    virtual protected void SetParent(P parent)
    {
        if (_parent is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initilized.");

        _parent = parent;
    }

    void ISrcNodeIntializer<P>.SetParent(P parent)
        => SetParent(parent);
}

public abstract class BaseSrcNode(string id) : BaseSrcNode<ISrcNodeHolder>(id)
{
}
