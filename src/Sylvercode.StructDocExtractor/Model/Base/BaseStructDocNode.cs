using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocNode<TParent>(string id) :
    IStructDocNode<TParent>, IStructDocNodeInitializer<TParent>
    where TParent : class, IStructDocNodeHolder
{
    private TParent? _parent;
    virtual public TParent Parent
    {
        get => _parent ?? throw new InvalidOperationException($"{nameof(Parent)} has not yet been initialized.");
    }

    public virtual bool IsRoot => false;
    public virtual bool IsLeaf => true;

    public string Id { get; } = id;

    public string NodeSnippet() => ((IStructDocNode)this).DebugName;

    virtual protected void SetParent(TParent parent)
    {
        if (_parent is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initialized.");

        _parent = parent;
    }

    void IStructDocNodeInitializer<TParent>.SetParent(TParent parent)
        => SetParent(parent);
}

public abstract class BaseStructDocNode(string id) : BaseStructDocNode<IStructDocNodeHolder>(id)
{
}
