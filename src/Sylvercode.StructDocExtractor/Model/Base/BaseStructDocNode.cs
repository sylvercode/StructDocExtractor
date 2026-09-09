using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base providing shared identity, parent tracking, and initialization support to all structural nodes.</summary>
/// <typeparam name="TParent">The concrete parent holder type that can contain this node.</typeparam>
/// <remarks>
/// Implements both <see cref="IStructDocNode{TParent}"/> and <see cref="IStructDocNodeInitializer{TParent}"/>,
/// giving subclasses typed parent access and participation in the tree-initialization protocol.
/// </remarks>
public abstract class BaseStructDocNode<TParent>(string id) :
    IStructDocNode<TParent>, IStructDocNodeInitializer<TParent>
    where TParent : class, IStructDocNodeHolder
{
    private TParent? _parent;

    /// <summary>Gets the parent holder that contains this node; throws <see cref="InvalidOperationException"/> if the parent has not yet been set.</summary>
    virtual public TParent Parent
    {
        get => _parent ?? throw new InvalidOperationException($"{nameof(Parent)} has not yet been initialized.");
    }

    /// <summary>Gets a value indicating whether this node is the root, determined by reference equality with its parent.</summary>
    public virtual bool IsRoot => ReferenceEquals(this, _parent);

    /// <summary>Gets a value indicating whether this node has no children; returns <see langword="true"/> by default.</summary>
    public virtual bool IsLeaf => true;

    /// <summary>Gets or sets the unique identifier for this node.</summary>
    public string Id { get; init; } = id;

    /// <summary>Returns the debug name of this node as its snippet representation.</summary>
    /// <returns>A short string combining the type name and node identifier.</returns>
    public string NodeSnippet() => ((IStructDocNode)this).DebugName;

    /// <summary>Sets the parent holder for this node; throws <see cref="InvalidOperationException"/> if a parent has already been assigned.</summary>
    /// <param name="parent">The parent holder to assign to this node.</param>
    virtual protected void SetParent(TParent parent)
    {
        if (_parent is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initialized.");

        _parent = parent;
    }

    void IStructDocNodeInitializer<TParent>.SetParent(TParent parent)
        => SetParent(parent);
}

/// <summary>Non-generic convenience base for structural nodes whose parent type is the untyped <see cref="IStructDocNodeHolder"/>.</summary>
public abstract class BaseStructDocNode(string id) : BaseStructDocNode<IStructDocNodeHolder>(id)
{
}
