using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="UriBaseTranslater.Translate"/> swapping URI bases.</summary>
public class UriBaseTranslater_Translaste
{
    /// <summary>Verifies that a URI whose base matches the source base is correctly rebased to the store base.</summary>
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

    /// <summary>Verifies that a URI with a mismatched base throws an <see cref="ArgumentException"/>.</summary>
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
