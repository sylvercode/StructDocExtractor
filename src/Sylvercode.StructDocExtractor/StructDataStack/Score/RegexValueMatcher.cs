using System.Text.RegularExpressions;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class RegexValueMatcher(Regex[] regex) : IEquatable<string>
{
    public RegexValueMatcher(Regex regex)
        : this([regex])
    {
    }

    public RegexValueMatcher(IEnumerable<string> patterns) 
        : this(patterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToArray())
    {

    }

    public RegexValueMatcher(string pattern) 
        : this(new Regex(pattern, RegexOptions.IgnoreCase))
    {

    }

    public bool Equals(string? other) => other is not null && regex.Any(r => r.IsMatch(other));
}
