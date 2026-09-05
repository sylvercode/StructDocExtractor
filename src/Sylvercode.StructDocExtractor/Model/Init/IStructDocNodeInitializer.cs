namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Contract for a structural node that can have its parent assigned during tree initialization.</summary>
public interface IStructDocNodeInitializer : IStructDocNode
{
    /// <summary>Sets the parent node for this initializer.</summary>
    /// <param name="parent">The node to assign as the parent.</param>
    void SetParent(IStructDocNode parent);

    /// <summary>Validates that the given node is of the correct parent type, throwing if not.</summary>
    /// <param name="node">The node to validate as a compatible parent.</param>
    void CheckParentTypeOrThrow(IStructDocNode node);
}

/// <summary>Strongly-typed variant of <see cref="IStructDocNodeInitializer"/> that constrains parent assignment to a specific holder type.</summary>
/// <typeparam name="TParent">The required parent holder type for this node.</typeparam>
public interface IStructDocNodeInitializer<TParent> : IStructDocNodeInitializer
    where TParent : class, IStructDocNodeHolder
{
    /// <summary>Sets the strongly-typed parent holder for this node.</summary>
    /// <param name="parent">The typed parent holder to assign.</param>
    void SetParent(TParent parent);

    void IStructDocNodeInitializer.SetParent(IStructDocNode parent) =>
        SetParent(CastAsParentTypeOrThrow(parent));

    /// <summary>Casts <paramref name="node"/> to <typeparamref name="TParent"/>, throwing <see cref="ArgumentException"/> if the cast fails.</summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The node cast to <typeparamref name="TParent"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="node"/> is not of type <typeparamref name="TParent"/>.</exception>
    TParent CastAsParentTypeOrThrow(IStructDocNode node)
    {
        if (node is not TParent)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(TParent)}");
        return (TParent)node;
    }
    void IStructDocNodeInitializer.CheckParentTypeOrThrow(IStructDocNode node)
        => CastAsParentTypeOrThrow(node);
}
