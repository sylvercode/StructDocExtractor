using System.Globalization;

namespace Sylvercode.StructDocExtractor.Metadatas;

/// <summary>Immutable key/value pair representing a single metadata entry attached to a structural node.</summary>
/// <param name="name">The metadata key that identifies this entry.</param>
/// <param name="value">The raw value for this entry; may be <see langword="null"/>.</param>
public class Metadata(string name, object? value)
{
    /// <summary>Gets the name (key) that identifies this metadata entry.</summary>
    public string Name { get; } = name;

    /// <summary>Gets the raw value of this metadata entry.</summary>
    public object? Value { get; } = value;

    /// <summary>Returns the value converted to <typeparamref name="TValue"/> using invariant culture.</summary>
    /// <typeparam name="TValue">The target type for conversion.</typeparam>
    /// <returns>The converted value, or <see langword="null"/> if the value is <see langword="null"/>.</returns>
    public TValue? GetValue<TValue>() => (TValue?)Convert.ChangeType(Value, typeof(TValue), CultureInfo.InvariantCulture);

    /// <summary>Returns the value converted to a <see cref="string"/> using invariant culture.</summary>
    /// <returns>The string representation of <see cref="Value"/>, or <see langword="null"/> if the value is <see langword="null"/>.</returns>
    public string? GetStrValue() => (string?)Convert.ChangeType(Value, TypeCode.String, CultureInfo.InvariantCulture);
}
