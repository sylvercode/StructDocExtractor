using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class StaticDirUriTranslaterTests_Translate
{
    [Theory]
    [InlineData("https://www.example.com/test.html", "output", "output/test.html")]
    [InlineData("https://www.example.com/dir/test.html", "output", "output/test.html")]
    [InlineData("https://www.example.com/test.html", "", "test.html")]
    [InlineData("https://www.example.com/dir/test.html", "", "test.html")]
    public void WithValideUri_ShouldReturnRelativeUri(string input, string outputPath, string expected)
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
