namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Defines the contract for matching a set of string values against a criterion and returning a numeric match score.</summary>
public interface IValueMatcher
{
    /// <summary>Matches a single string value against this criterion and returns the score.</summary>
    /// <param name="test">The string to test.</param>
    /// <returns>A positive integer score when the value matches; zero indicates no match.</returns>
    public int Match(string test) => Match([test]);

    /// <summary>Matches a collection of string values against this criterion and returns the score.</summary>
    /// <param name="values">The values to test.</param>
    /// <returns>A positive integer score when the values satisfy this matcher; zero indicates no match.</returns>
    public int Match(IEnumerable<string> values);
}
