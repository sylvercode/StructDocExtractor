using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceProcessorProviderTests_GetProcessor
{
    private static ResourceProcessorMock ImageProcessor { get; } = new();

    private static ResourceProcessorMock BaseProcessor { get; } = new();

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
