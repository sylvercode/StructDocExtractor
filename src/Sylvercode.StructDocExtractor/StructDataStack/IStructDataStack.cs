using System.Diagnostics;

namespace Sylvercode.StructDocExtractor.StructDataStack;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IStructDataStack<TDiscriminator> : IEnumerable<IStructDataStack<TDiscriminator>.Entry>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, TDiscriminator nodeDiscriminator)
    {
        public int Depth { get; } = depth;
        public TDiscriminator NodeDiscriminator { get; } = nodeDiscriminator;

        private string GetDebuggerDisplay()
        {
            return $"{Depth}: {NodeDiscriminator}";
        }

        public override string ToString() => NodeDiscriminator?.ToString() ?? string.Empty;
    }

    public Entry Peek();
    public int Count { get; }
}
