using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests;

public class RegexValueMatcher_WithSingelValue
{
    [Theory]
    [InlineData("foo")]
    [InlineData("foofoo")]
    [InlineData("foo1")]
    public void Match_WithSameValue_ReturnTrue(string input)
    {
        // Given
        RegexValueMatcher matcher = new("foo.*");

        // When
        bool result = matcher.Equals(input);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData("goo")]
    [InlineData("fo1")]
    public void Match_WithDiffertValue_ReturnsFalse(string input)
    {
        // Given
        RegexValueMatcher matcher = new("foo.*");

        // When
        bool result = matcher.Equals(input);

        // Then
        Assert.False(result);
    }

}

public class RegexValueMatcher_WithMultiValues
{
    [Theory]
    [InlineData("one")]
    [InlineData("ony")]
    [InlineData("twofive")]
    [InlineData("foo1")]
    [InlineData("oo1")]
    public void Match_WithOneOfTheValues_ReturnTrue(string input)
    {
        // Given
        RegexValueMatcher matcher = new("on[ey]", "two.*", ".*oo.?");

        // When
        bool result = matcher.Equals(input);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData("on")]
    [InlineData("five")]
    [InlineData("on7")]
    public void Match_WithNoneOfTheValues_ReturnsFalse(string input)
    {
        // Given
        RegexValueMatcher matcher = new("on[ey]", "two.*", ".*oo.?");

        // When
        bool result = matcher.Equals(input);

        // Then
        Assert.False(result);
    }
}
