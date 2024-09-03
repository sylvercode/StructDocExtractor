using System.Diagnostics;

namespace Sylvercode.StructDocExtractor.Model.Utils;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IStructDocNodeStack : IEnumerable<IStructDocNodeStack.Entry>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, IStructDocNode node)
    {
        public int Depth => depth;
        public IStructDocNode Node => node;

        private string GetDebuggerDisplay() => $"{Depth}: {Node.Id}";
    }

    public Entry? Peek();
    public int Count { get; }
}
