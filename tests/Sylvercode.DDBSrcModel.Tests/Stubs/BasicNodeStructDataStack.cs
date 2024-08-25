using System.Collections;
using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Tests.Stubs;

public class BasicNodeStructDataStack : IStructDataStack<BasicNodeSelectable>
{
    private readonly Stack<IStructDataStack<BasicNodeSelectable>.Entry> _entries = [];

    public int Count => _entries.Count;

    public IStructDataStack<BasicNodeSelectable>.Entry Peek() => _entries.Peek();

    public IEnumerator<IStructDataStack<BasicNodeSelectable>.Entry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

    public void Push(IStructDataStack<BasicNodeSelectable>.Entry entry) => _entries.Push(entry);

    public IStructDataStack<BasicNodeSelectable>.Entry Pop() => _entries.Pop();

    public void Push(BasicNodeSelectable node) => Push(new IStructDataStack<BasicNodeSelectable>.Entry(Count, node));
}
