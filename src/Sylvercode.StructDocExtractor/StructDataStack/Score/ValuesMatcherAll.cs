namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Composite <see cref="IValueMatcher"/> that requires every inner matcher to find a match across the tested values (logical AND).</summary>
public class ValuesMatcherAll(IEquatable<string>[] valueMatchers, bool skipUnmatchValues = true) : IValueMatcher
{
    /// <summary>Gets the inner matchers that must all be satisfied.</summary>
    public IEnumerable<IEquatable<string>> ValueMatchers => valueMatchers;

    /// <summary>Gets whether values that do not match any inner matcher are silently skipped.</summary>
    public bool SkipUnmatchValues => skipUnmatchValues;

    /// <summary>Initializes a new instance of <see cref="ValuesMatcherAll"/> with a single matcher.</summary>
    /// <param name="valueMatcher">The single matcher that must find a match.</param>
    /// <param name="skipUnmatchValues">When <see langword="true"/>, values that match no inner matcher are skipped; otherwise, an unmatched value returns zero immediately.</param>
    public ValuesMatcherAll(IEquatable<string> valueMatcher, bool skipUnmatchValues = true)
            : this([valueMatcher], skipUnmatchValues)
    {

    }

    /// <inheritdoc/>
    public int Match(IEnumerable<string> values)
    {
        if (valueMatchers.Length == 0)
            return 0;

        LinkedList<IEquatable<string>> matchersList = new(valueMatchers);
        foreach (var value in values)
        {
            var matcher = matchersList.First;
            for (; matcher is not null; matcher = matcher.Next)
            {
                if (matcher.Value.Equals(value))
                {
                    matchersList.Remove(matcher);
                    break;
                }
            }

            if (matcher is null && !skipUnmatchValues)
                return 0;

            if (matchersList.Count == 0)
                break;
        }

        return matchersList.Count == 0 ? valueMatchers.Length : 0;
    }
}
