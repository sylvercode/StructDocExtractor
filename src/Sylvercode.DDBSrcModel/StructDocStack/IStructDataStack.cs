using System.Diagnostics;

namespace Sylvercode.DDBSrcModel.StructDocStack;

public interface IStructDataStack<N> : IEnumerable<IStructDataStack<N>.Entry>
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, N nodeSelectable)
    {
        public int Depth { get; } = depth;
        public N NodeSelectable { get; } = nodeSelectable;

        private string GetDebuggerDisplay()
        {
            return $"{Depth}: {NodeSelectable}";
        }

        public override string ToString() => NodeSelectable?.ToString() ?? string.Empty;
    }

    public Entry Peek();
    public int Count { get; }
}
