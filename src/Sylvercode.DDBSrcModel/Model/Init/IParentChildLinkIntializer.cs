namespace Sylvercode.DDBSrcModel.Model.Init;

public interface IParentChildLinkIntializer
{
    void InitializeParentChildLink();
    void AddChild(ISrcNodeIntializer child);
}

public interface IParentChildLinkIntializer<in C> : IParentChildLinkIntializer
    where C : class, ISrcNode
{
    void AddChild<O>(O child)
        where O : class, C, ISrcNodeIntializer =>
        ((IParentChildLinkIntializer)this).AddChild(child);

}
