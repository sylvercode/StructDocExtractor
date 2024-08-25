namespace Sylvercode.DDBSrcModel.StructDocStack;

public interface IStructDataStack<N> : IEnumerable<IStructDataStack<N>.Entry>
{
    public readonly struct Entry(int depth, N nodeSelectable)
    {
        public int Depth { get; } = depth;
        public N NodeSelectable { get; } = nodeSelectable;
    }

    public Entry Peek();
    public int Count { get; }
}
