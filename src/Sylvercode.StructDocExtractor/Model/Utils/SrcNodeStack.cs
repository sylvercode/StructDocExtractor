using System.Collections;

namespace Sylvercode.StructDocExtractor.Model.Utils;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class StructDocNodeStack : IStructDocNodeStack
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Stack<IStructDocNodeStack.Entry> _entries = [];

    public StructDocNodeStack(IEnumerable<IStructDocNode> nodes)
    {
        foreach (var node in nodes)
            Push(node);
    }

    public int Count => _entries.Count;

    public IEnumerator<IStructDocNodeStack.Entry> GetEnumerator()
        => _entries.GetEnumerator();

    public IStructDocNodeStack.Entry? Peek()
        => _entries.Peek();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Push(IStructDocNode Node) => _entries.Push(new IStructDocNodeStack.Entry(Count, Node));

    public IStructDocNodeStack.Entry Pop() => _entries.Pop();
}
