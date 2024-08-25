using System.Text.RegularExpressions;

namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public class RegexValueMatcher(params Regex[] regex) : IEquatable<string>
{
    public RegexValueMatcher(params string[] patterns) : this(patterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToArray())
    {

    }

    public bool Equals(string? other) => other is not null && regex.Any(r => r.IsMatch(other));
}
