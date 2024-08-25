using System.Diagnostics;

namespace Sylvercode.DDBSrcModel.Model.Utils;

public interface ISrcNodeStack : IEnumerable<ISrcNodeStack.Entry>
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, ISrcNode node)
    {
        public int Depth => depth;
        public ISrcNode Node => node;

        private string GetDebuggerDisplay() => $"{Depth}: {Node.Id}";
    }

    public Entry? Peek();
    public int Count { get; }
}
