using Sylvercode.SiteFetcher.UriTransformer;

namespace Sylvercode.SiteFetcher.Tests;

public class ProxyUriTransformerTests_Transform
{
    [Fact]
    public void ValidBase_ReturnsValidUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri proxyBase = new("https://proxy.com/");
        Uri uri = new("https://example.com/test");

        ProxyChangeBaseUriTransformer transformer = new(sourceBase, proxyBase);

        // When
        Uri result = transformer.Transform(uri);

        // Then
        Assert.Equal("https://proxy.com/test", result.ToString());
    }

    [Fact]
    public void InvalidBase_WithOtherBaseAsError_ThrowsArgumentException()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri proxyBase = new("https://proxy.com/");
        Uri uri = new("https://example.org/test");

        ProxyChangeBaseUriTransformer transformer = new(sourceBase, proxyBase, new() { OtherBaseAsError = true });

        // When
        void action() => transformer.Transform(uri);

        // Then
        ArgumentException ex = Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void InvalidBase_WithoutOtherBaseAsError_ReturnsUri()
    {
        // Given
        Uri sourceBase = new("https://example.com/");
        Uri proxyBase = new("https://proxy.com/");
        Uri uri = new("https://example.org/test");

        ProxyChangeBaseUriTransformer transformer = new(sourceBase, proxyBase);

        // When
        Uri result = transformer.Transform(uri);

        // Then
        Assert.Equal(uri.ToString(), result.ToString());
    }
}
