namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Strongly-typed contract for initializing a node holder with a typed child content collection.</summary>
/// <typeparam name="TChild">The type of child nodes this holder accepts.</typeparam>
public interface IStructDocNodeHolderInitializer<TChild> :
    IStructDocNodeHolderInitializer
    where TChild : class, IStructDocNode
{
    /// <summary>Sets the typed child content collection for this holder.</summary>
    /// <param name="content">The child nodes to assign.</param>
    void SetContent(IEnumerable<TChild> content);

    void IStructDocNodeHolderInitializer.SetContent(IEnumerable<IStructDocNode> content) =>
        SetContent(content.Select(CastAsChildTypeOrThrow));

    /// <summary>Creates a new typed parent/child link initializer scoped to this holder.</summary>
    /// <returns>A new <see cref="IParentChildLinkInitializer{TChild}"/> for this holder.</returns>
    new IParentChildLinkInitializer<TChild> NewParentChildLinkInitializer();

    IParentChildLinkInitializer IStructDocNodeHolderInitializer.NewParentChildLinkInitializer() =>
        NewParentChildLinkInitializer();

    /// <summary>Casts <paramref name="node"/> to <typeparamref name="TChild"/>, throwing <see cref="ArgumentException"/> if the cast fails.</summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The node cast to <typeparamref name="TChild"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="node"/> is not of type <typeparamref name="TChild"/>.</exception>
    TChild CastAsChildTypeOrThrow(IStructDocNode node)
    {
        if (node is not TChild)
            throw new ArgumentException($"Parameter {nameof(node)} is not of type {nameof(TChild)}");
        return (TChild)node;
    }
    void IStructDocNodeHolderInitializer.CheckChildTypeOrThrow(IStructDocNode node)
        => CheckChildTypeOrThrow(node);
}

/// <summary>Base contract for a node holder that accepts typed content assignment and link initialization during tree construction.</summary>
public interface IStructDocNodeHolderInitializer : IStructDocNode
{
    /// <summary>Sets the untyped child content for this holder.</summary>
    /// <param name="content">The child nodes to assign.</param>
    void SetContent(IEnumerable<IStructDocNode> content);

    /// <summary>Initializes this holder's content to an empty collection.</summary>
    void InitContentAsEmpty();

    /// <summary>Creates a new link initializer for establishing parent/child relationships on this holder.</summary>
    /// <returns>A new <see cref="IParentChildLinkInitializer"/> for this holder.</returns>
    IParentChildLinkInitializer NewParentChildLinkInitializer();

    /// <summary>Validates that the given node is of the accepted child type, throwing if not.</summary>
    /// <param name="node">The node to validate as a compatible child.</param>
    void CheckChildTypeOrThrow(IStructDocNode node);
}
