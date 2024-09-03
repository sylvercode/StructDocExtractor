namespace Sylvercode.StructDocExtractor.Model.Init;

public interface IParentChildLinkInitializer
{
    void AddChild(IStructDocNodeInitializer child);
    IEnumerable<IStructDocNode> ChildrenToAdd();
    void InitializeParentChildLink();
}

public interface IParentChildLinkInitializer<TChild> : IParentChildLinkInitializer
    where TChild : class, IStructDocNode
{
    void AddChild<TOtherChild>(TOtherChild child)
        where TOtherChild : class, TChild, IStructDocNodeInitializer =>
        ((IParentChildLinkInitializer)this).AddChild(child);
    new IEnumerable<TChild> ChildrenToAdd();
    IEnumerable<IStructDocNode> IParentChildLinkInitializer.ChildrenToAdd() => ChildrenToAdd();
}
