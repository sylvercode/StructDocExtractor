namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class ValuesMatcherAny(IEquatable<string>[] valueMatchers, bool firstOnly = true) : IValueMatcher
{
    public ValuesMatcherAny(IEquatable<string> valueMatchers, bool firstOnly = true) : this([valueMatchers], firstOnly)
    {
    }

    public int Match(IEnumerable<string> values)
    {
        if (firstOnly)
            return valueMatchers.Any(m => values.Any(v => m.Equals(v))) ? 1 : 0;
        else
            return valueMatchers.Count(m => values.Any(v => m.Equals(v)));
    }
}
