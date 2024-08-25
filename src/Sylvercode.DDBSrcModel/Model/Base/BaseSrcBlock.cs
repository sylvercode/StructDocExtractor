using System.Collections.Immutable;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcBlock<P, C>(string id) :
    BaseSrcNode<P>(id), ISrcBlock<P, C>, ISrcNodeHolderInitializer<C>
    where P : class, ISrcNodeHolder
    where C : class, ISrcNode
{
    private IReadOnlyList<C>? _content;
    public IReadOnlyList<C> Content
    {
        get => _content ?? [];
    }

    IParentChildLinkIntializer<C> ISrcNodeHolderInitializer<C>.NewParentChildLinkIntializer()
    {
        return new ParentChildLinkIntializer<ISrcNodeHolderInitializer<C>, C>(this);
    }

    void ISrcNodeHolderInitializer.InitContentAsEmpty()
        => SetContent([]);

    virtual protected void SetContent(IEnumerable<C> content)
    {
        if (_content is not null)
            throw new InvalidOperationException($"{nameof(Parent)} has been already initilized.");

        _content = ImmutableList.CreateRange(content);
    }

    void ISrcNodeHolderInitializer<C>.SetContent(IEnumerable<C> content)
        => SetContent(content);
}
