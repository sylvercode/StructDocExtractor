using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Markdown.Tests;

public class MarkdownUriTranslaterTests_Translate
{
    private static IHost CreateDefaultHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.Configure<SiteExtractorOptions>(options =>
                {
                    options.SourceAuthority = "https://example.com";
                    options.SourceBasePath = "/source";
                })
                .AddSingleton<MarkdownUriTranslater>();
            }).Build();
    }

    [Theory]
    [InlineData("https://example.com/source/page.html", "page.md")]
    [InlineData("https://example.com/source/dir/page.html", "dir/page.md")]
    [InlineData("https://example.com/source/page", "page.md")]
    [InlineData("https://example.com/source/page.html#frag", "page.md#frag")]
    [InlineData("https://example.com/source/page#frag", "page.md#frag")]
    [InlineData("https://example.com/source/page.html#frag?key1=value&keu2=value", "page.md#frag?key1=value&keu2=value")]
    [InlineData("https://example.com/source/page#frag?key1=value&keu2=value", "page.md#frag?key1=value&keu2=value")]
    [InlineData("https://example.com/source/dir/page#frag?key1=value&keu2=value", "dir/page.md#frag?key1=value&keu2=value")]
    public void NoPageHeading_BaseRenameOnly(string input, string expected)
    {
        // Given
        IHost host = CreateDefaultHost();
        Resource resource = new(new Uri(input));
        var translater = host.Services.GetRequiredService<MarkdownUriTranslater>();

        // When
        Uri result = translater.Translate(resource, resource.Uri);

        // Then
        Assert.Equal(expected, Uri.UnescapeDataString(result.ToString()));
    }

    [Theory]
    [InlineData("https://example.com/source/page.html", "Page Title", "Page Title.md")]
    [InlineData("https://example.com/source/page.html#frag", "Page Title", "Page Title.md#frag")]
    [InlineData(
        "https://example.com/source/page%20with%20space.html?key1=value&keu2=value#frag",
        "Page Title",
        "Page Title.md?key1=value&keu2=value#frag")]
    public void WithPageHeading_NameWithPageHeading(string input, string heading, string expected)
    {
        // Given
        IHost host = CreateDefaultHost();
        Resource resource = new(new Uri(input));
        resource.Metadata.AddMetadata(StdMetadata.PageTopHeadingKey, heading);
        var translater = host.Services.GetRequiredService<MarkdownUriTranslater>();

        // When
        Uri result = translater.Translate(resource, resource.Uri);

        // Then
        Assert.Equal(expected, Uri.UnescapeDataString(result.ToString()));
    }
}
