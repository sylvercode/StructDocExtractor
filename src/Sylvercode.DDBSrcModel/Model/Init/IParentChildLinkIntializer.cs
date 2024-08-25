namespace Sylvercode.DDBSrcModel.Model.Init;

public interface IParentChildLinkIntializer
{
    void AddChild(ISrcNodeIntializer child);
    IEnumerable<ISrcNode> ChildrenToAdd();
    void InitializeParentChildLink();
}

public interface IParentChildLinkIntializer<C> : IParentChildLinkIntializer
    where C : class, ISrcNode
{
    void AddChild<O>(O child)
        where O : class, C, ISrcNodeIntializer =>
        ((IParentChildLinkIntializer)this).AddChild(child);
    new IEnumerable<C> ChildrenToAdd();
    IEnumerable<ISrcNode> IParentChildLinkIntializer.ChildrenToAdd() => ChildrenToAdd();
}
