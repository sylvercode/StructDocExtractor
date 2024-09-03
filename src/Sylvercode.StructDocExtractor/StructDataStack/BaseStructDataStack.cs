using System.Collections;

namespace Sylvercode.StructDocExtractor.StructDataStack;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class BaseStructDataStack<TDiscriminator>() : IStructDataStack<TDiscriminator>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Stack<IStructDataStack<TDiscriminator>.Entry> _entries = [];

    public BaseStructDataStack(IEnumerable<TDiscriminator> nodes) : this()
    {
        foreach (var node in nodes)
            Push(node);
    }

    public int Count => _entries.Count;

    public IStructDataStack<TDiscriminator>.Entry Peek() => _entries.Peek();

    public IEnumerator<IStructDataStack<TDiscriminator>.Entry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

    public void Push(TDiscriminator Node) => _entries.Push(new IStructDataStack<TDiscriminator>.Entry(Count, Node));

    public IStructDataStack<TDiscriminator>.Entry Pop() => _entries.Pop();

    public override string ToString()
        => string.Join(", ", "`" + _entries + "`");
}
