using System.Text.RegularExpressions;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Value matcher that tests a string against one or more compiled regular expressions using case-insensitive matching.</summary>
public class RegexValueMatcher(Regex[] regex) : IEquatable<string>
{
    /// <summary>Gets the compiled regular expressions used by this matcher.</summary>
    public IEnumerable<Regex> Regex => regex;

    /// <summary>Initializes a new instance of <see cref="RegexValueMatcher"/> with a single pre-compiled regular expression.</summary>
    /// <param name="regex">The compiled regular expression to match against.</param>
    public RegexValueMatcher(Regex regex)
        : this([regex])
    {
    }

    /// <summary>Initializes a new instance of <see cref="RegexValueMatcher"/> from a collection of pattern strings.</summary>
    /// <param name="patterns">The regular expression patterns to compile; matching is case-insensitive.</param>
    public RegexValueMatcher(IEnumerable<string> patterns)
        : this(patterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToArray())
    {

    }

    /// <summary>Initializes a new instance of <see cref="RegexValueMatcher"/> from a single pattern string.</summary>
    /// <param name="pattern">The regular expression pattern to compile; matching is case-insensitive.</param>
    public RegexValueMatcher(string pattern)
        : this(new Regex(pattern, RegexOptions.IgnoreCase))
    {

    }

    /// <summary>Determines whether the specified string matches any of the compiled regular expressions.</summary>
    /// <param name="other">The string to test.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> is matched by at least one expression; otherwise, <see langword="false"/>.</returns>
    public bool Equals(string? other) => other is not null && regex.Any(r => r.IsMatch(other));
}
