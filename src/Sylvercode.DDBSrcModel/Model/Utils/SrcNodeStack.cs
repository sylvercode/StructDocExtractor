using System.Collections;

namespace Sylvercode.DDBSrcModel.Model.Utils;

public class SrcNodeStack : ISrcNodeStack
{
    private readonly Stack<ISrcNodeStack.Entry> _entries = [];

    public SrcNodeStack(IEnumerable<ISrcNode> nodes)
    {
        foreach (var node in nodes)
            Push(node);
    }

    public int Count => _entries.Count;

    public IEnumerator<ISrcNodeStack.Entry> GetEnumerator()
        => _entries.GetEnumerator();

    public ISrcNodeStack.Entry? Peek()
        => _entries.Peek();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Push(ISrcNode Node) => _entries.Push(new ISrcNodeStack.Entry(Count, Node));

    public ISrcNodeStack.Entry Pop() => _entries.Pop();
}
