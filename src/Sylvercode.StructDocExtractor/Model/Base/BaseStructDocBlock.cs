using System.Collections.Immutable;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for block-level nodes that manage a typed, immutable child content list and participate in the holder initialization protocol.</summary>
/// <typeparam name="TParent">The type of the parent node holder.</typeparam>
/// <typeparam name="TChild">The type of child nodes stored in this block's content list.</typeparam>
/// <remarks>
/// Implements <see cref="IStructDocNodeHolderInitializer{TChild}"/> so that child nodes and parent/child links
/// can be established atomically during tree construction via <see cref="ParentChildLinkInitializer{TParent,TChild}"/>.
/// </remarks>
public abstract class BaseStructDocBlock<TParent, TChild>(string id) :
    BaseStructDocNode<TParent>(id), IStructDocBlock<TParent, TChild>, IStructDocNodeHolderInitializer<TChild>
    where TParent : class, IStructDocNodeHolder
    where TChild : class, IStructDocNode
{
    private IReadOnlyList<TChild>? _content;

    /// <summary>Gets the immutable list of child nodes; returns an empty list if the content has not yet been initialized.</summary>
    public IReadOnlyList<TChild> Content
    {
        get => _content ?? [];
    }

    /// <summary>Gets a value indicating whether the content has been set and contains at least one child node.</summary>
    public bool HasContent => _content is not null && _content.Count > 0;

    IParentChildLinkInitializer<TChild> IStructDocNodeHolderInitializer<TChild>.NewParentChildLinkInitializer()
    {
        return new ParentChildLinkInitializer<IStructDocNodeHolderInitializer<TChild>, TChild>(this);
    }

    void IStructDocNodeHolderInitializer.InitContentAsEmpty()
        => SetContent([]);

    /// <summary>Stores the child content as an immutable list; throws <see cref="InvalidOperationException"/> if content has already been set.</summary>
    /// <param name="content">The child nodes to assign to this block.</param>
    virtual protected void SetContent(IEnumerable<TChild> content)
    {
        if (_content is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initialized.");

        _content = ImmutableList.CreateRange(content);
    }

    void IStructDocNodeHolderInitializer<TChild>.SetContent(IEnumerable<TChild> content)
        => SetContent(content);
}
