using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class UriBaseTranslater_Translaste
{
    [Fact]
    public void ValidBase_ReturnsValidUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri storeBase = new("https://proxy.com/");
        Uri uri = new("https://example.com/test");

        UriBaseTranslater transformer = new(sourceBase, storeBase);

        // When
        Uri result = transformer.Translate(uri);

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

        UriBaseTranslater transformer = new(sourceBase, storeBase);

        // When
        void action() => transformer.Translate(uri);

        // Then
        ArgumentException ex = Assert.Throws<ArgumentException>(action);
    }
}
