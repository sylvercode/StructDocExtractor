using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.StdHtml.Tests.Stubs;

namespace Sylvercode.SiteExtractor.StdHtml.Tests;

public class HtmlResourceDependencyFilterTetsts_Accepted
{
    private const string SourceAuthority = "https://example.com";
    private const string SourceBasePath = "/base/";
    private const string SourceBaseUri = SourceAuthority + SourceBasePath;
    private const string OtherSourceAuthority = "https://other.com";
    private const string OtherSourceBaseUri = OtherSourceAuthority + SourceBasePath;
    private static IHost GetHost(HtmlResourceDependencyFilterMode mode)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddHtmlResourceDependencyFilter();
                services.Configure<SiteExtractorOptions>(o =>
                {
                    o.SourceAuthority = SourceAuthority;
                    o.SourceBasePath = SourceBasePath;
                });
                services.Configure<HtmlResourceDependencyFilterOptions>(
                    o =>
                    {
                        o.ExternalFilterMode = mode;
                    }
                );

            })
            .Build();
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test.jpg")]
    public void InSourceBase(string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(SourceBaseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test.jpg")]
    public void NotInSourceBase(string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(OtherSourceBaseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test.jpg")]
    [InlineData(SourceBaseUri, "test.png")]
    [InlineData(OtherSourceBaseUri, "test.jpg")]
    [InlineData(OtherSourceBaseUri, "test.png")]
    public void IsImageWithImage(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.IsImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test")]
    [InlineData(OtherSourceBaseUri, "test")]
    [InlineData(SourceBaseUri, "test.txt")]
    [InlineData(OtherSourceBaseUri, "test.txt")]
    public void IsImageWithoutImage(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.IsImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test.jpg")]
    [InlineData(SourceBaseUri, "test.png")]
    [InlineData(OtherSourceBaseUri, "test.jpg")]
    [InlineData(OtherSourceBaseUri, "test.png")]
    public void NotIsImageWithImage(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.IsNotImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test")]
    [InlineData(OtherSourceBaseUri, "test")]
    [InlineData(SourceBaseUri, "test.txt")]
    [InlineData(OtherSourceBaseUri, "test.txt")]
    public void NotIsImageWithoutImage(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.IsNotImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test")]
    [InlineData(OtherSourceBaseUri, "test")]
    [InlineData(SourceBaseUri, "test.txt")]
    [InlineData(OtherSourceBaseUri, "test.txt")]
    [InlineData(SourceBaseUri, "test.jpg")]
    [InlineData(SourceBaseUri, "test.png")]
    [InlineData(OtherSourceBaseUri, "test.jpg")]
    [InlineData(OtherSourceBaseUri, "test.png")]
    public void NoneWithAnything(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.None);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test.jpg")]
    [InlineData(SourceBaseUri, "test.png")]
    public void IsImageInSourceBaseWithValid(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData(OtherSourceBaseUri, "test.jpg")]
    [InlineData(OtherSourceBaseUri, "test.png")]
    [InlineData(SourceBaseUri, "test.txt")]
    [InlineData(SourceBaseUri, "test")]
    public void IsImageInSourceBaseWithInvalid(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }

    [Theory]
    [InlineData(SourceBaseUri, "test")]
    [InlineData(SourceBaseUri, "test.txt")]
    public void IsNotImageInSourceBaseWithValid(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsNotImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.True(result);
    }

    [Theory]
    [InlineData(OtherSourceBaseUri, "test.txt")]
    [InlineData(OtherSourceBaseUri, "test")]
    [InlineData(SourceBaseUri, "test.jpg")]
    [InlineData(SourceBaseUri, "test.png")]
    public void IsNotImageInSourceBaseWithInvalid(string baseUri, string endUri)
    {
        // Given
        IHost host = GetHost(HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsNotImage);
        IResourceDependancyFilter filter = host.Services.GetRequiredService<IResourceDependancyFilter>();
        StubReferencer referencer = new(baseUri + endUri);

        // When
        bool result = filter.IsAccepted(referencer);

        // Then
        Assert.False(result);
    }
}
