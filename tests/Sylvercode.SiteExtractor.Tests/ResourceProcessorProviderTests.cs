using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourceProcessorProvider.GetProcessor"/> resolution by MIME type and URI.</summary>
public class ResourceProcessorProviderTests_GetProcessor
{
    private static ResourceProcessorMock ImageProcessor { get; } = new();

    private static ResourceProcessorMock BaseProcessor { get; } = new();

    /// <summary>Creates a default DI host with image and base-URI processors registered in priority order.</summary>
    /// <returns>A built <see cref="IHost"/> ready for test use.</returns>
    public static IHost GetDefaultHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddResourceProcessor(ImageProcessor, ImageUriMatcher.Default);
                services.AddResourceProcessor(BaseProcessor, new UriMatcherByBase(new Uri("https://example.com")));
                services.AddResourceProcessorProvider();
            })
            .Build();
    }

    /// <summary>Verifies that an image URI resolves to the registered image processor.</summary>
    [Fact]
    public void GetImageProcessor_AddProcessor()
    {
        // Given
        IHost host = GetDefaultHost();
        var provider = host.Services.GetRequiredService<IResourceProcessorProvider>();

        // When
        var processor = provider.GetProcessor(new Uri("https://example.com/image.jpg"));

        // Then
        Assert.Same(ImageProcessor, processor);
    }

    /// <summary>Verifies that a non-image URI within the registered base resolves to the base processor.</summary>
    [Fact]
    public void GetBaseProcessor_AddProcessor()
    {
        // Given
        IHost host = GetDefaultHost();
        var provider = host.Services.GetRequiredService<IResourceProcessorProvider>();

        // When
        var processor = provider.GetProcessor(new Uri("https://example.com/resource"));

        // Then
        Assert.Same(BaseProcessor, processor);
    }

    /// <summary>Verifies that a URI matching no registered processor returns <see langword="null"/>.</summary>
    [Fact]
    public void GetNoProcessor_ReturnsNull()
    {
        // Given
        IHost host = GetDefaultHost();
        var provider = host.Services.GetRequiredService<IResourceProcessorProvider>();

        // When
        var processor = provider.GetProcessor(new Uri("https://other.com/resource"));

        // Then
        Assert.Null(processor);
    }
}
