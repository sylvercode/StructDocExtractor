using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests;

public class StaticValueMatcher_WithSingelValue
{
    [Fact]
    public void Match_WithSameValue_ReturnTrue()
    {
        // Given
        const string foo = nameof(foo);
        StaticValueMatcher matcher = new(foo);

        // When
        bool result = matcher.Equals(foo);

        // Then
        Assert.True(result);
    }

    [Fact]
    public void Match_WithDiffertValue_ReturnsFalse()
    {
        // Given
        const string foo = nameof(foo);
        StaticValueMatcher matcher = new(foo);

        // When
        bool result = matcher.Equals(foo + foo);

        // Then
        Assert.False(result);
    }

}

public class StaticValueMatcher_WithMultiValues
{
    [Fact]
    public void Match_WithOneOfTheValues_ReturnTrue()
    {
        // Given
        const string foo1 = nameof(foo1);
        const string foo2 = nameof(foo2);
        const string foo3 = nameof(foo3);
        StaticValueMatcher matcher = new([foo1, foo2, foo3]);

        // When
        bool result = matcher.Equals(foo2);

        // Then
        Assert.True(result);
    }

    [Fact]
    public void Match_WithNoneOfTheValues_ReturnsFalse()
    {
        // Given
        const string foo1 = nameof(foo1);
        const string foo2 = nameof(foo2);
        const string foo3 = nameof(foo3);
        StaticValueMatcher matcher = new([foo1, foo2, foo3]);

        // When
        bool result = matcher.Equals(foo1 + foo2);

        // Then
        Assert.False(result);
    }
}
