using System.Collections;
using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicNodeStructDataStack() : IStructDataStack<BasicNodeDiscriminator>
{
    public BasicNodeStructDataStack(BasicNodeDiscriminator singleEntry) : this() => Push(singleEntry);
    private readonly Stack<IStructDataStack<BasicNodeDiscriminator>.Entry> _entries = [];

    public int Count => _entries.Count;

    public IStructDataStack<BasicNodeDiscriminator>.Entry Peek() => _entries.Peek();

    public IEnumerator<IStructDataStack<BasicNodeDiscriminator>.Entry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

    public void Push(IStructDataStack<BasicNodeDiscriminator>.Entry entry) => _entries.Push(entry);

    public IStructDataStack<BasicNodeDiscriminator>.Entry Pop() => _entries.Pop();

    public void Push(BasicNodeDiscriminator node) => Push(new IStructDataStack<BasicNodeDiscriminator>.Entry(Count, node));
}
