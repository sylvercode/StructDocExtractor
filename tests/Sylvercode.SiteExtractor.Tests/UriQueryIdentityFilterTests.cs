using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for source-identity query filtering policies.</summary>
public class UriQueryIdentityFilterTests_Filter
{
    [Fact]
    public void RemoveAll_RemovesQueryAndPreservesFragment()
    {
        Uri result = new RemoveAllUriQueryIdentityFilter().Filter(
            new Uri("https://example.com/page?lang=en#section"));

        Assert.Equal("https://example.com/page#section", result.AbsoluteUri);
    }

    [Fact]
    public void PreserveAll_KeepsQueryAndFragment()
    {
        Uri source = new("https://example.com/page?lang=en#section");

        Uri result = new PreserveAllUriQueryIdentityFilter().Filter(source);

        Assert.Equal(source, result);
    }

    [Theory]
    [InlineData("page?mode=1", "page")]
    [InlineData("page?mode=1#section", "page#section")]
    [InlineData("page#section?mode=1", "page#section?mode=1")]
    [InlineData("page#section", "page#section")]
    public void RemoveAll_RelativeUriPreservesFragment(
        string input,
        string expected)
    {
        Uri result = new RemoveAllUriQueryIdentityFilter().Filter(
            new Uri(input, UriKind.Relative));

        Assert.Equal(expected, result.OriginalString);
    }
}
