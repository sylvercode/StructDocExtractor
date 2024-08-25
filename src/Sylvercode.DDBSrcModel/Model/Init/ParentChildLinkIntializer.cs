
namespace Sylvercode.DDBSrcModel.Model.Init;

public class ParentChildLinkIntializer<P, C>(P parent) :
    IParentChildLinkIntializer<C>
    where P : class, ISrcNodeHolderInitializer<C>
    where C : class, ISrcNode
{
    private readonly List<C> _childrens = [];

    public void AddChild(ISrcNodeIntializer child)
    {
        child.CheckParentTypeOrThrow(parent);
        _childrens.Add(parent.CastAsChildTypeOrThrow(child));
    }

    public IEnumerable<C> ChildrenToAdd()
        => _childrens;

    public void InitializeParentChildLink()
    {
        parent.SetContent(_childrens);
        _childrens.ForEach(c => ((ISrcNodeIntializer)c).SetParent(parent));
    }
}
