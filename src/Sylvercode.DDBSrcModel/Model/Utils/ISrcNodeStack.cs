namespace Sylvercode.DDBSrcModel.Model.Utils;

public interface ISrcNodeStack : IEnumerable<ISrcNodeStack.Entry>
{
    public readonly struct Entry(int depth, ISrcNode node)
    {
        public int Depth => depth;
        public ISrcNode Node => node;
    }

    public Entry? Peek();
    public int Count { get; }
}
