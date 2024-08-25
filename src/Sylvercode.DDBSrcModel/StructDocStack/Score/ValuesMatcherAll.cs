namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public class ValuesMatcherAll(IEquatable<string>[] valueMatchers, bool skipUnmatchValues = true) : IValueMatcher
{
    public ValuesMatcherAll(IEquatable<string> valueMatcher, bool skipUnmatchValues = true)
            : this([valueMatcher], skipUnmatchValues)
    {

    }

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
