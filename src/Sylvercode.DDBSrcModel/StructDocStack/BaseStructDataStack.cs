using System.Collections;

namespace Sylvercode.DDBSrcModel.StructDocStack;

public class BaseStructDataStack<N>() : IStructDataStack<N>
{
    private readonly Stack<IStructDataStack<N>.Entry> _entries = [];

    public BaseStructDataStack(IEnumerable<N> nodes) : this()
    {
        foreach (var node in nodes)
            Push(node);
    }

    public int Count => _entries.Count;

    public IStructDataStack<N>.Entry Peek() => _entries.Peek();

    public IEnumerator<IStructDataStack<N>.Entry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

    public void Push(N Node) => _entries.Push(new IStructDataStack<N>.Entry(Count, Node));

    public IStructDataStack<N>.Entry Pop() => _entries.Pop();
}
