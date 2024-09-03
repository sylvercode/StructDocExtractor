using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests;

public class ValuesMatcherAll_WithSkipUnmatchValuesFalse
{
    [Theory]
    [InlineData(3, new string[] { "goo", "foo", "hoo" })]
    [InlineData(3, new string[] { "foo", "goo", "hoo" })]
    [InlineData(3, new string[] { "foo", "hoo", "goo" })]
    [InlineData(3, new string[] { "hoo", "goo", "foo" })]
    [InlineData(3, new string[] { "hoo", "foo", "goo" })]
    [InlineData(3, new string[] { "goo", "hoo", "foo" })]
    public void Match_WithSameValues_(int three, string[] input)
    {
        // Given
        ValuesMatcherAll matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], skipUnmatchValues: false);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(three, result);
    }

    [Theory]
    [InlineData(0, new string[] { "goo", "foo" })]
    [InlineData(0, new string[] { "foo", "goo", "eoo" })]
    [InlineData(0, new string[] { "hoo", "goo", })]
    [InlineData(0, new string[] { "goo", "eoo", "foo" })]
    public void Match_WithMissingValues_ReturnZero(int count, string[] input)
    {
        // Given
        ValuesMatcherAll matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], skipUnmatchValues: false);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(count, result);
    }

}

public class ValuesMatcherAll_WithSkipUnmatchValuesTrue
{
    [Theory]
    [InlineData(3, new string[] { "goo", "foo", "hoo", "ioo" })]
    [InlineData(3, new string[] { "foo", "goo", "ioo", "hoo" })]
    [InlineData(3, new string[] { "foo", "hoo", "goo", "ioo" })]
    [InlineData(3, new string[] { "hoo", "ioo", "goo", "foo" })]
    [InlineData(3, new string[] { "hoo", "ioo", "foo", "goo" })]
    [InlineData(3, new string[] { "goo", "hoo", "ioo", "foo" })]
    public void Match_WithSameValues_ReturnCount(int three, string[] input)
    {
        // Given
        ValuesMatcherAll matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], skipUnmatchValues: true);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(three, result);
    }
    [Theory]
    [InlineData(0, new string[] { "goo", "aoo", "hoo", "ioo" })]
    [InlineData(0, new string[] { "aoo", "goo", "ioo", "hoo" })]
    [InlineData(0, new string[] { "foo", "hoo", "aoo", "ioo" })]
    [InlineData(0, new string[] { "hoo", "ioo", "goo", "aoo" })]
    [InlineData(0, new string[] { "hoo", "ioo", "foo", "aoo" })]
    [InlineData(0, new string[] { "goo", "aoo", "ioo", "foo" })]
    public void Match_WithMissingValues_ReturnZero(int zero, string[] input)
    {
        // Given
        ValuesMatcherAll matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], skipUnmatchValues: true);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(zero, result);
    }
}
