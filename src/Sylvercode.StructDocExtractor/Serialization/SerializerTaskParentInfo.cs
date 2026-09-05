namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Carries parent-context information for a <see cref="SerializerTask"/>, linking it to its parent task and adjacent siblings.</summary>
public class SerializerTaskParentInfo(SerializerTask parent)
{
    /// <summary>Gets the parent <see cref="SerializerTask"/> that owns this child context.</summary>
    public SerializerTask Parent { get; } = parent;

    /// <summary>Gets or sets the previous sibling task, or <see langword="null"/> if this task is the first child.</summary>
    public SerializerTask? PreviousSibling { get; set; }

    /// <summary>Gets or sets the next sibling task, or <see langword="null"/> if this task is the last child.</summary>
    public SerializerTask? NextSibling { get; set; }

    /// <summary>Gets a value indicating whether this task is the first child of its parent.</summary>
    public bool IsFirstChild => PreviousSibling is null;

    /// <summary>Gets a value indicating whether this task is the last child of its parent.</summary>
    public bool IsLastChild => NextSibling is null;
}
