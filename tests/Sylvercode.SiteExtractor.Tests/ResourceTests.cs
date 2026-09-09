using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="Resource.TranslateUri"/> producing correct output URIs.</summary>
public class ResourceTests_TranslateUri
{
    /// <summary>Verifies that translating a URI with no fragment changes only the base to the configured output base.</summary>
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

    /// <summary>Verifies that translating a URI with a fragment changes the base and preserves the fragment.</summary>
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

    /// <summary>Verifies that translating a URI whose base does not match the resource URI throws an <see cref="ArgumentException"/>.</summary>
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
