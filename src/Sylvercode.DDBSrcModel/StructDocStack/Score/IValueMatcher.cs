namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public interface IValueMatcher
{
    public int Match(string test) => Match([test]);
    public int Match(IEnumerable<string> test);
}
