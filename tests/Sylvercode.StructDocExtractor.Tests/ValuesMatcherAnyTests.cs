using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests;

public class MultiValuesMatchAny_WithFirstOnly
{
    [Theory]
    [InlineData(1, new string[] { "foo" })]
    [InlineData(1, new string[] { "eoo", "foo" })]
    [InlineData(1, new string[] { "eoo", "foo", "goo" })]
    public void Match_WithSameValue_ReturnOne(int one, string[] input)
    {
        // Given
        ValuesMatcherAny matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], firstOnly: true);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(one, result);
    }

    [Theory]
    [InlineData(1, new string[] { "foo" })]
    [InlineData(1, new string[] { "eoo", "foo" })]
    [InlineData(2, new string[] { "eoo", "foo", "goo" })]
    public void Match_WithSameValue_ReturnCount(int count, string[] input)
    {
        // Given
        ValuesMatcherAny matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], firstOnly: false);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(count, result);
    }

    [Theory]
    [InlineData(0, new string[] { "eoo" })]
    [InlineData(0, new string[] { "aoo", "doo" })]
    public void Match_WithDiffentValue_ReturnZero(int count, string[] input)
    {
        // Given
        ValuesMatcherAny matcher = new([new StaticValueMatcher("foo"), new StaticValueMatcher("goo"), new StaticValueMatcher("hoo")], firstOnly: false);

        // When
        int result = matcher.Match(input);

        // Then
        Assert.Equal(count, result);
    }

}
