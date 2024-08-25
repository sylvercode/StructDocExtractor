
namespace Sylvercode.DDBSrcModel.Model.Init;

public class ParentChildLinkIntializer<P, C>(P parent) :
    IParentChildLinkIntializer<C>
    where P : class, ISrcNodeHolderInitializer<C>
    where C : class, ISrcNode
{
    private List<C>? _childrens = [];

    public void AddChild(ISrcNodeIntializer child)
    {
        if (_childrens is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        child.CheckParentTypeOrThrow(parent);
        _childrens.Add(parent.CastAsChildTypeOrThrow(child));
    }

    public IEnumerable<C> ChildrenToAdd()
        => _childrens ?? Enumerable.Empty<C>();

    public void InitializeParentChildLink()
    {
        if (_childrens is null)
            throw new InvalidOperationException($"{nameof(InitializeParentChildLink)} Have been already called.");

        parent.SetContent(_childrens);
        _childrens.ForEach(c => ((ISrcNodeIntializer)c).SetParent(parent));
        _childrens = null;
    }
}
