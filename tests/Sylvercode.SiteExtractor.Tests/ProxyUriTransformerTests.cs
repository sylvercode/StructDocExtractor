using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class UriBaseTranslaterTests_Translaste
{
    [Fact]
    public void ValidBase_ReturnsValidUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri storeBase = new("https://proxy.com/");
        Uri uri = new("https://example.com/test");

        UriBaseTranslater transformer = new();

        // When
        Uri result = transformer.Translate(uri, storeBase, sourceBase);

        // Then
        Assert.Equal("https://proxy.com/test", result.ToString());
    }

    [Fact]
    public void InvalidBase_WithOtherBaseAsError_ThrowsArgumentException()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri storeBase = new("https://proxy.com/");
        Uri uri = new("https://example.org/test");

        UriBaseTranslater transformer = new(new() { OtherBaseAsError = true });

        // When
        void action() => transformer.Translate(uri, storeBase, sourceBase);

        // Then
        ArgumentException ex = Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void InvalidBase_WithoutOtherBaseAsError_ReturnsUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri storeBase = new("https://proxy.com/");
        Uri uri = new("https://example.org/test");

        UriBaseTranslater transformer = new();

        // When
        Uri result = transformer.Translate(uri, storeBase, sourceBase);

        // Then
        Assert.Equal(uri.ToString(), result.ToString());
    }
}
