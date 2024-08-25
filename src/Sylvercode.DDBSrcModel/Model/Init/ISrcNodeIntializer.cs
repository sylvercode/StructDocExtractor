namespace Sylvercode.DDBSrcModel.Model.Init;

public interface ISrcNodeIntializer : ISrcNode
{
    void SetParent(ISrcNode parent);

    void CheckParentTypeOrThrow(ISrcNode node);
}

public interface ISrcNodeIntializer<P> : ISrcNodeIntializer
    where P : class, ISrcNodeHolder
{
    void SetParent(P parent);
    void ISrcNodeIntializer.SetParent(ISrcNode parent) =>
        SetParent(CastAsParentTypeOrThrow(parent));

    P CastAsParentTypeOrThrow(ISrcNode node)
    {
        if (node is not P)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(P)}");
        return (P)node;
    }
    void ISrcNodeIntializer.CheckParentTypeOrThrow(ISrcNode node)
        => CastAsParentTypeOrThrow(node);
}
