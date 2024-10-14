using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceTests_TranslateUri
{
    [Fact]
    public void NoFragmentUrl_ResultOnlyBaseChange()
    {
        // Given
        Uri sourceUri = new("https://example.com/resource");
        Resource resource = new(sourceUri)
        {
            UriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"))
        };

        // When
        Uri result = resource.TranslateUri(sourceUri);

        // Then
        Assert.Equal(new Uri("https://test.com/resource"), result);
    }

    [Fact]
    public void WithFragmentUrl_ResultBaseChangePlusFragment()
    {
        // Given
        Uri sourceUri = new("https://example.com/resource");
        Resource resource = new(sourceUri)
        {
            UriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"))
        };

        // When
        Uri result = resource.TranslateUri(new Uri("https://example.com/resource#fragment"));

        // Then
        Assert.Equal(new Uri("https://test.com/resource#fragment"), result);
    }

    [Fact]
    public void WrongBase_Throw()
    {
        // Given
        Uri sourceUri = new("https://example.com/resource");
        Resource resource = new(sourceUri)
        {
            UriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"))
        };

        // When
        void act() => resource.TranslateUri(new Uri("https://wrong.com/resource#fragment"));

        // Then
        Assert.Throws<ArgumentException>(act);
    }
}
