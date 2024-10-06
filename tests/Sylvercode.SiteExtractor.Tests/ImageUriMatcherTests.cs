using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class ImageUriMatcherTests_IsMatching
{
    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("https://example.com/image.jpeg")]
    [InlineData("https://example.com/image.png")]
    [InlineData("https://example.com/image.gif")]
    [InlineData("https://example.com/image.bmp")]
    [InlineData("https://example.com/image.webp")]
    [InlineData("https://example.com/image.svg")]
    public void ValidExtention_ReturnTrue(string uri)
    {
        // Given
        ImageUriMatcher matcher = new();

        // When
        bool result = matcher.IsMatching(new Uri(uri));

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData("https://example.com/image.txt")]
    [InlineData("https://example.com/image")]
    public void InvalidExtention_ReturnFalse(string uri)
    {
        // Given
        ImageUriMatcher matcher = new();

        // When
        bool result = matcher.IsMatching(new Uri(uri));

        // Then
        Assert.False(result);
    }
}
