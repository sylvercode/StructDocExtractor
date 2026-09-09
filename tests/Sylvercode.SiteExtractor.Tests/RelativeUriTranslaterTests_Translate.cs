using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="RelativeUriTranslater.Translate"/> resolving relative URIs against a base.</summary>
public class RelativeUriTranslaterTests_Translate
{
    /// <summary>Verifies that a valid URI is translated to a relative path beneath the configured output subdirectory.</summary>
    [Theory]
    [InlineData("https://www.example.com/test.html", "output", "output/test.html")]
    [InlineData("https://www.example.com/dir/test.html", "output", "dir/output/test.html")]
    [InlineData("https://www.example.com/test.html", "", "test.html")]
    [InlineData("https://www.example.com/dir/test.html", "", "dir/test.html")]
    [InlineData("https://www.example.com/dir1/dir2/test.html", "out", "dir1/dir2/out/test.html")]
    [InlineData("https://www.example.com/dir1/dir2/test.html", "", "dir1/dir2/test.html")]
    public void WithValideUri_ReturnsRelativeToSource(string input, string outputPath, string expected)
    {
        // Arrange
        var uri = new Uri(input);
        var translater = new RelativeUriTranslater(new Uri("https://www.example.com/"), outputPath, logger: null);

        // Act
        var result = translater.Translate(uri);

        // Assert
        Assert.Equal(expected, result.ToString());
    }
}
