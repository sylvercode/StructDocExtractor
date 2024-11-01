using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class UriRelativeFromBaseTranslater_Translaste
{
    [Fact]
    public void ValidBase_ReturnsValidUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri uri = new("https://example.com/test");

        UriRelativeFromBaseTranslater transformer = new(sourceBase);

        // When
        Uri result = transformer.Translate(uri);

        // Then
        Assert.Equal("test", result.ToString());
    }

    [Fact]
    public void InvalidBase_WithOtherBaseAsError_ThrowsArgumentException()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri uri = new("https://example.org/test");

        UriRelativeFromBaseTranslater transformer = new(sourceBase);

        // When
        void action() => transformer.Translate(uri);

        // Then
        ArgumentException ex = Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void ValidBase_ReturnsValidUriWithQuery()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri uri = new("https://example.com/test?query=1");

        UriRelativeFromBaseTranslater transformer = new(sourceBase);

        // When
        Uri result = transformer.Translate(uri);

        // Then
        Assert.Equal("test?query=1", result.ToString());
    }

    [Fact]
    public void ValidBase_ReturnsValidUriWithFragment()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri uri = new("https://example.com/test#fragment");

        UriRelativeFromBaseTranslater transformer = new(sourceBase);

        // When
        Uri result = transformer.Translate(uri);

        // Then
        Assert.Equal("test#fragment", result.ToString());
    }

    [Fact]
    public void ValidBase_ReturnsValidUriWithQueryAndFragment()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri uri = new("https://example.com/test?query=1#fragment");

        UriRelativeFromBaseTranslater transformer = new(sourceBase);

        // When
        Uri result = transformer.Translate(uri);

        // Then
        Assert.Equal("test?query=1#fragment", result.ToString());
    }
}
