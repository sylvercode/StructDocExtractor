using System.Diagnostics;

namespace Sylvercode.StructDocExtractor.Model.Utils;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Contract for a stack that tracks structural document nodes and their depth during document traversal.</summary>
public interface IStructDocNodeStack : IEnumerable<IStructDocNodeStack.Entry>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    /// <summary>Represents a single stack entry pairing a structural node with its traversal depth.</summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, IStructDocNode node)
    {
        /// <summary>Gets the traversal depth of this entry.</summary>
        public int Depth => depth;

        /// <summary>Gets the structural node associated with this entry.</summary>
        public IStructDocNode Node => node;

        private string GetDebuggerDisplay() => $"{Depth}: {Node.Id}";
    }

    /// <summary>Returns the top entry without removing it, or <see langword="null"/> if the stack is empty.</summary>
    /// <returns>The top <see cref="Entry"/>, or <see langword="null"/> when empty.</returns>
    public Entry? Peek();

    /// <summary>Gets the number of entries currently in the stack.</summary>
    public int Count { get; }
}
