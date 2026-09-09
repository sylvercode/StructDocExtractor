using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ImageUriMatcher.IsMatching"/> against various image and non-image URIs.</summary>
public class ImageUriMatcherTests_IsMatching
{
    /// <summary>Verifies that URIs with recognised image extensions are matched as images.</summary>
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

    /// <summary>Verifies that URIs without recognised image extensions are not matched as images.</summary>
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
