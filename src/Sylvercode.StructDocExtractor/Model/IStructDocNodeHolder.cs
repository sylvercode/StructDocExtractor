namespace Sylvercode.StructDocExtractor.Model;

/// <summary>Contract for nodes that hold an ordered collection of child <see cref="IStructDocNode"/> instances.</summary>
public interface IStructDocNodeHolder : IStructDocNode
{
    /// <summary>Gets the ordered read-only list of child nodes held by this node.</summary>
    IReadOnlyList<IStructDocNode> Content { get; }

    /// <summary>Gets a value indicating whether this holder contains at least one child node.</summary>
    bool HasContent { get; }
}

/// <summary>Strongly-typed holder contract that constrains the child node collection to a specific type.</summary>
/// <typeparam name="TChild">The concrete type of child nodes held by this node.</typeparam>
public interface IStructDocNodeHolder<out TChild> : IStructDocNodeHolder
    where TChild : class, IStructDocNode
{
    /// <summary>Gets the strongly-typed read-only list of child nodes held by this node.</summary>
    new IReadOnlyList<TChild> Content { get; }

    IReadOnlyList<IStructDocNode> IStructDocNodeHolder.Content => Content;
}
