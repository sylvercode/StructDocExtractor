namespace Sylvercode.StructDocExtractor.Model.Init;

/// <summary>Contract for setting up bi-directional parent/child links between a holder node and its children.</summary>
public interface IParentChildLinkInitializer
{
    /// <summary>Registers a child node to be linked to the parent holder during initialization.</summary>
    /// <param name="child">The child node initializer to register.</param>
    void AddChild(IStructDocNodeInitializer child);

    /// <summary>Returns the children registered for linking so far.</summary>
    /// <returns>An enumerable of nodes awaiting parent/child link setup.</returns>
    IEnumerable<IStructDocNode> ChildrenToAdd();

    /// <summary>Commits the registered children by assigning content to the holder and setting the parent on each child.</summary>
    void InitializeParentChildLink();
}

/// <summary>Strongly-typed variant of <see cref="IParentChildLinkInitializer"/> for typed child collections.</summary>
/// <typeparam name="TChild">The type of child nodes managed by this initializer.</typeparam>
public interface IParentChildLinkInitializer<TChild> : IParentChildLinkInitializer
    where TChild : class, IStructDocNode
{
    /// <summary>Registers a typed child that is both <typeparamref name="TChild"/> and <see cref="IStructDocNodeInitializer"/>.</summary>
    /// <typeparam name="TOtherChild">The concrete child type, which must satisfy both constraints.</typeparam>
    /// <param name="child">The child node to register.</param>
    void AddChild<TOtherChild>(TOtherChild child)
        where TOtherChild : class, TChild, IStructDocNodeInitializer =>
        ((IParentChildLinkInitializer)this).AddChild(child);

    /// <summary>Returns the typed children registered for linking so far.</summary>
    /// <returns>An enumerable of typed child nodes awaiting parent/child link setup.</returns>
    new IEnumerable<TChild> ChildrenToAdd();
    IEnumerable<IStructDocNode> IParentChildLinkInitializer.ChildrenToAdd() => ChildrenToAdd();
}
