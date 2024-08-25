namespace Sylvercode.DDBSrcModel.Model.Init;

public interface ISrcNodeHolderInitializer<C> :
    ISrcNodeHolderInitializer
    where C : class, ISrcNode
{
    void SetContent(IEnumerable<C> content);
    void ISrcNodeHolderInitializer.SetContent(IEnumerable<ISrcNode> content) =>
        SetContent(content.Select(CastAsChildTypeOrThrow));

    new IParentChildLinkIntializer<C> NewParentChildLinkIntializer();

    IParentChildLinkIntializer ISrcNodeHolderInitializer.NewParentChildLinkIntializer() =>
        NewParentChildLinkIntializer();

    C CastAsChildTypeOrThrow(ISrcNode node)
    {
        if (node is not C)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(C)}");
        return (C)node;
    }
    void ISrcNodeHolderInitializer.CheckChildTypeOrThrow(ISrcNode node)
        => CheckChildTypeOrThrow(node);
}

public interface ISrcNodeHolderInitializer : ISrcNode
{
    void SetContent(IEnumerable<ISrcNode> content);
    void InitContentAsEmpty();
    IParentChildLinkIntializer NewParentChildLinkIntializer();

    void CheckChildTypeOrThrow(ISrcNode node);
}
