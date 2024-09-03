namespace Sylvercode.StructDocExtractor.Model.Init;

public interface IStructDocNodeHolderInitializer<TChild> :
    IStructDocNodeHolderInitializer
    where TChild : class, IStructDocNode
{
    void SetContent(IEnumerable<TChild> content);
    void IStructDocNodeHolderInitializer.SetContent(IEnumerable<IStructDocNode> content) =>
        SetContent(content.Select(CastAsChildTypeOrThrow));

    new IParentChildLinkInitializer<TChild> NewParentChildLinkInitializer();

    IParentChildLinkInitializer IStructDocNodeHolderInitializer.NewParentChildLinkInitializer() =>
        NewParentChildLinkInitializer();

    TChild CastAsChildTypeOrThrow(IStructDocNode node)
    {
        if (node is not TChild)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(TChild)}");
        return (TChild)node;
    }
    void IStructDocNodeHolderInitializer.CheckChildTypeOrThrow(IStructDocNode node)
        => CheckChildTypeOrThrow(node);
}

public interface IStructDocNodeHolderInitializer : IStructDocNode
{
    void SetContent(IEnumerable<IStructDocNode> content);
    void InitContentAsEmpty();
    IParentChildLinkInitializer NewParentChildLinkInitializer();

    void CheckChildTypeOrThrow(IStructDocNode node);
}
