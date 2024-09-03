namespace Sylvercode.StructDocExtractor.Model.Init;

public interface IStructDocNodeInitializer : IStructDocNode
{
    void SetParent(IStructDocNode parent);

    void CheckParentTypeOrThrow(IStructDocNode node);
}

public interface IStructDocNodeInitializer<TParent> : IStructDocNodeInitializer
    where TParent : class, IStructDocNodeHolder
{
    void SetParent(TParent parent);
    void IStructDocNodeInitializer.SetParent(IStructDocNode parent) =>
        SetParent(CastAsParentTypeOrThrow(parent));

    TParent CastAsParentTypeOrThrow(IStructDocNode node)
    {
        if (node is not TParent)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(TParent)}");
        return (TParent)node;
    }
    void IStructDocNodeInitializer.CheckParentTypeOrThrow(IStructDocNode node)
        => CastAsParentTypeOrThrow(node);
}
