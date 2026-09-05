using System.Collections;

namespace Sylvercode.StructDocExtractor.StructDataStack;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Concrete base implementation of <see cref="IStructDataStack{TDiscriminator}"/> providing push, pop, and enumeration over stacked discriminator entries.</summary>
/// <typeparam name="TDiscriminator">The discriminator type associated with each stacked entry.</typeparam>
public class BaseStructDataStack<TDiscriminator>() : IStructDataStack<TDiscriminator>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Stack<IStructDataStack<TDiscriminator>.Entry> _entries = [];

    /// <summary>Initializes a new instance of <see cref="BaseStructDataStack{TDiscriminator}"/> pre-populated from <paramref name="nodes"/>.</summary>
    /// <param name="nodes">The discriminator values to push onto the stack in enumeration order.</param>
    public BaseStructDataStack(IEnumerable<TDiscriminator> nodes) : this()
    {
        foreach (var node in nodes)
            Push(node);
    }

    /// <inheritdoc/>
    public int Count => _entries.Count;

    /// <inheritdoc/>
    public IStructDataStack<TDiscriminator>.Entry Peek() => _entries.Peek();

    /// <inheritdoc/>
    public IEnumerator<IStructDataStack<TDiscriminator>.Entry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

    /// <summary>Pushes a new discriminator entry onto the top of the stack at the current depth.</summary>
    /// <param name="Node">The discriminator value to push.</param>
    public void Push(TDiscriminator Node) => _entries.Push(new IStructDataStack<TDiscriminator>.Entry(Count, Node));

    /// <summary>Removes and returns the entry at the top of the stack.</summary>
    /// <returns>The removed <see cref="IStructDataStack{TDiscriminator}.Entry"/>.</returns>
    public IStructDataStack<TDiscriminator>.Entry Pop() => _entries.Pop();

    /// <inheritdoc/>
    public override string ToString()
        => string.Join(", ", "`" + _entries + "`");
}
