namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class StaticValueMatcher(params string[] values) : IEquatable<string>
{
    public bool Equals(string? other) => other is not null && values.Contains(other);
}
