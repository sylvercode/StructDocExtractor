using System.Diagnostics;

namespace Sylvercode.StructDocExtractor.StructDataStack;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Defines the contract for a typed, enumerable stack of structural data entries used during discriminated node extraction.</summary>
/// <typeparam name="TDiscriminator">The discriminator type associated with each stacked entry.</typeparam>
public interface IStructDataStack<TDiscriminator> : IEnumerable<IStructDataStack<TDiscriminator>.Entry>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    /// <summary>Represents a single entry on the structural data stack, pairing a depth level with its associated node discriminator.</summary>
    /// <param name="depth">The zero-based depth at which this entry was pushed.</param>
    /// <param name="nodeDiscriminator">The discriminator value for this entry.</param>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly struct Entry(int depth, TDiscriminator nodeDiscriminator)
    {
        /// <summary>Gets the zero-based depth of this entry within the stack at the time of insertion.</summary>
        public int Depth { get; } = depth;

        /// <summary>Gets the discriminator value associated with this stack entry.</summary>
        public TDiscriminator NodeDiscriminator { get; } = nodeDiscriminator;

        private string GetDebuggerDisplay()
        {
            return $"{Depth}: {NodeDiscriminator}";
        }

        /// <inheritdoc/>
        public override string ToString() => NodeDiscriminator?.ToString() ?? string.Empty;
    }

    /// <summary>Returns the entry at the top of the stack without removing it.</summary>
    /// <returns>The topmost <see cref="Entry"/> in the stack.</returns>
    public Entry Peek();

    /// <summary>Gets the number of entries currently in the stack.</summary>
    public int Count { get; }
}
