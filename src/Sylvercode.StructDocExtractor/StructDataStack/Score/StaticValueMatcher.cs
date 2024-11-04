namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class StaticValueMatcher(string[] values) : IEquatable<string>
{
    public StaticValueMatcher(string value) : this([value]) { }
    public bool Equals(string? other) => other is not null && values.Contains(other);
}
