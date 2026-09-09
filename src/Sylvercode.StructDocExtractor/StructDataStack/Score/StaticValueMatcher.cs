namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Value matcher that compares a string against a fixed set of constant values using equality.</summary>
public class StaticValueMatcher(string[] values) : IEquatable<string>
{
    /// <summary>Gets the set of constant string values that this matcher accepts.</summary>
    public IEnumerable<string> Values => values;

    /// <summary>Initializes a new instance of <see cref="StaticValueMatcher"/> with a single accepted value.</summary>
    /// <param name="value">The constant string value that this matcher will accept.</param>
    public StaticValueMatcher(string value) : this([value]) { }

    /// <summary>Determines whether the specified string equals any of the accepted values.</summary>
    /// <param name="other">The string to test.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> is contained in the accepted values; otherwise, <see langword="false"/>.</returns>
    public bool Equals(string? other) => other is not null && values.Contains(other);
}
