using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="StaticDirUriTranslater.Translate"/> mapping URIs to local paths.</summary>
public class StaticDirUriTranslaterTests_Translate
{
    /// <summary>Verifies that a valid URI is translated to the configured output directory regardless of its path depth.</summary>
    [Theory]
    [InlineData("https://www.example.com/test.html", "output", "output/test.html")]
    [InlineData("https://www.example.com/dir/test.html", "output", "output/test.html")]
    [InlineData("https://www.example.com/test.html", "", "test.html")]
    [InlineData("https://www.example.com/dir/test.html", "", "test.html")]
    public void WithValideUri_ReturnConfigureDir(string input, string outputPath, string expected)
    {
        // Arrange
        var uri = new Uri(input);
        var translater = new StaticDirUriTranslater(outputPath, logger: null);

        // Act
        var result = translater.Translate(uri);

        // Assert
        Assert.Equal(expected, result.ToString());
    }
}
