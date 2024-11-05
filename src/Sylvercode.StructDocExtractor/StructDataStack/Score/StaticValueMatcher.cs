namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class StaticValueMatcher(string[] values) : IEquatable<string>
{
    public IEnumerable<string> Values => values;

    public StaticValueMatcher(string value) : this([value]) { }
    public bool Equals(string? other) => other is not null && values.Contains(other);
}
