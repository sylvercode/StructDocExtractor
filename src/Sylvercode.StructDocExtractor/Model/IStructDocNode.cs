namespace Sylvercode.StructDocExtractor.Model;

/// <summary>Defines the base contract for every node in the structured document tree.</summary>
public interface IStructDocNode
{
    /// <summary>Gets the parent holder that contains this node.</summary>
    IStructDocNodeHolder Parent { get; }

    /// <summary>Gets or sets the unique identifier for this node.</summary>
    string Id { get; init; }

    /// <summary>Gets a value indicating whether this node is the root of the document tree.</summary>
    bool IsRoot { get; }

    /// <summary>Gets a value indicating whether this node has no children.</summary>
    bool IsLeaf { get; }

    /// <summary>Gets a human-readable debug label combining the type name and node identifier.</summary>
    string DebugName => $"{GetType().Name}({Id})";

    /// <summary>Returns a short text snippet representing this node's content for diagnostics.</summary>
    /// <returns>A brief string representation of this node.</returns>
    string NodeSnippet();
}

/// <summary>Strongly-typed variant of <see cref="IStructDocNode"/> that constrains the parent holder to a specific type.</summary>
/// <typeparam name="TParent">The concrete parent holder type for this node.</typeparam>
public interface IStructDocNode<out TParent> : IStructDocNode
    where TParent : class, IStructDocNodeHolder
{
    /// <summary>Gets the strongly-typed parent holder that contains this node.</summary>
    new TParent Parent { get; }
    IStructDocNodeHolder IStructDocNode.Parent { get => Parent; }
}
