namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for root-level container blocks that are their own parent and reject external parent assignment.</summary>
/// <typeparam name="TChild">The type of child nodes this root block can contain.</typeparam>
/// <remarks>
/// Overrides <see cref="BaseStructDocNode{TParent}.IsRoot"/> to always return <see langword="true"/> and returns
/// <see langword="this"/> from <see cref="BaseStructDocNode{TParent}.Parent"/>, enforcing the invariant that root nodes have no external parent.
/// </remarks>
public abstract class BaseStructDocRootBlock<TChild>(string id) : BaseStructDocBlockWithAnyParent<TChild>(id)
    where TChild : class, IStructDocNode
{
    /// <summary>Gets a value indicating whether this node is the root; always returns <see langword="true"/>.</summary>
    override public bool IsRoot => true;

    /// <summary>Gets the parent holder; always returns this node itself to satisfy the self-referential root invariant.</summary>
    override public IStructDocNodeHolder Parent
    {
        get => this;
    }

    /// <summary>Always throws <see cref="InvalidOperationException"/>; root nodes cannot be assigned an external parent.</summary>
    /// <param name="parent">Unused.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    override protected void SetParent(IStructDocNodeHolder parent)
    {
        throw new InvalidOperationException($"Root node cannot set parent");
    }
}
