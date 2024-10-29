using System.Collections.Immutable;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocBlock<TParent, TChild>(string id) :
    BaseStructDocNode<TParent>(id), IStructDocBlock<TParent, TChild>, IStructDocNodeHolderInitializer<TChild>
    where TParent : class, IStructDocNodeHolder
    where TChild : class, IStructDocNode
{
    private IReadOnlyList<TChild>? _content;
    public IReadOnlyList<TChild> Content
    {
        get => _content ?? [];
    }

    public bool HasContent => _content is not null && _content.Count > 0;

    IParentChildLinkInitializer<TChild> IStructDocNodeHolderInitializer<TChild>.NewParentChildLinkInitializer()
    {
        return new ParentChildLinkInitializer<IStructDocNodeHolderInitializer<TChild>, TChild>(this);
    }

    void IStructDocNodeHolderInitializer.InitContentAsEmpty()
        => SetContent([]);

    virtual protected void SetContent(IEnumerable<TChild> content)
    {
        if (_content is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initialized.");

        _content = ImmutableList.CreateRange(content);
    }

    void IStructDocNodeHolderInitializer<TChild>.SetContent(IEnumerable<TChild> content)
        => SetContent(content);
}
