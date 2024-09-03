namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public interface IValueMatcher
{
    public int Match(string test) => Match([test]);
    public int Match(IEnumerable<string> values);
}
