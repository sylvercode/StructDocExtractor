namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Composite <see cref="IValueMatcher"/> that passes when at least one inner matcher finds a match across the tested values (logical OR).</summary>
public class ValuesMatcherAny(IEquatable<string>[] valueMatchers, bool firstOnly = true) : IValueMatcher
{
    /// <summary>Gets the inner matchers, any of which may satisfy this criterion.</summary>
    public IEnumerable<IEquatable<string>> ValueMatchers => valueMatchers;

    /// <summary>Gets whether the matcher stops and returns 1 on the first match, or counts all matching matchers.</summary>
    public bool FirstOnly => firstOnly;

    /// <summary>Initializes a new instance of <see cref="ValuesMatcherAny"/> with a single matcher.</summary>
    /// <param name="valueMatchers">The single matcher to test against the values.</param>
    /// <param name="firstOnly">When <see langword="true"/>, returns 1 on the first match; when <see langword="false"/>, counts all matching matchers.</param>
    public ValuesMatcherAny(IEquatable<string> valueMatchers, bool firstOnly = true) : this([valueMatchers], firstOnly)
    {
    }

    /// <inheritdoc/>
    public int Match(IEnumerable<string> values)
    {
        if (firstOnly)
            return valueMatchers.Any(m => values.Any(v => m.Equals(v))) ? 1 : 0;
        else
            return valueMatchers.Count(m => values.Any(v => m.Equals(v)));
    }
}
