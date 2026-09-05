using System.Collections;

namespace Sylvercode.StructDocExtractor.Model.Utils;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Stack implementation that maintains structural document nodes with their traversal depth during document processing.</summary>
public class StructDocNodeStack : IStructDocNodeStack
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Stack<IStructDocNodeStack.Entry> _entries = [];

    /// <summary>Initializes a new instance of <see cref="StructDocNodeStack"/> by pushing each node in <paramref name="nodes"/> in order.</summary>
    /// <param name="nodes">The initial nodes to push onto the stack.</param>
    public StructDocNodeStack(IEnumerable<IStructDocNode> nodes)
    {
        foreach (var node in nodes)
            Push(node);
    }

    /// <inheritdoc/>
    public int Count => _entries.Count;

    /// <inheritdoc/>
    public IEnumerator<IStructDocNodeStack.Entry> GetEnumerator()
        => _entries.GetEnumerator();

    /// <inheritdoc/>
    public IStructDocNodeStack.Entry? Peek()
        => _entries.Peek();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>Pushes <paramref name="Node"/> onto the stack, assigning the current <see cref="Count"/> as its depth.</summary>
    /// <param name="Node">The structural node to push.</param>
    public void Push(IStructDocNode Node) => _entries.Push(new IStructDocNodeStack.Entry(Count, Node));

    /// <summary>Removes and returns the top entry from the stack.</summary>
    /// <returns>The top <see cref="IStructDocNodeStack.Entry"/>.</returns>
    public IStructDocNodeStack.Entry Pop() => _entries.Pop();
}
