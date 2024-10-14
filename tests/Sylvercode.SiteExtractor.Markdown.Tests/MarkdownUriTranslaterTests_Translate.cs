using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources;

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
                    options.OutputDirectory = "/output";
                })
                .AddSingleton<MarkdownUriTranslater>();
            }).Build();
    }

    [Theory]
    [InlineData("https://example.com/source/page.html", "file:///output/page.md")]
    [InlineData("https://example.com/source/page", "file:///output/page.md")]
    [InlineData("https://example.com/source/page.html#frag", "file:///output/page.md#frag")]
    [InlineData("https://example.com/source/page#frag", "file:///output/page.md#frag")]
    [InlineData("https://example.com/source/page.html#frag?key1=value&keu2=value", "file:///output/page.md#frag?key1=value&keu2=value")]
    [InlineData("https://example.com/source/page#frag?key1=value&keu2=value", "file:///output/page.md#frag?key1=value&keu2=value")]
    public void NoPageHeading_BaseRenameOnly(string input, string expected)
    {
        // Given
        IHost host = CreateDefaultHost();
        Resource resource = new(new Uri(input));
        var translater = host.Services.GetRequiredService<MarkdownUriTranslater>();

        // When
        Uri result = translater.Translate(resource, resource.Uri);

        // Then
        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("https://example.com/source/page.html", "Page Title", "file:///output/Page Title.md")]
    [InlineData("https://example.com/source/page.html#frag", "Page Title", "file:///output/Page Title.md#frag")]
    [InlineData(
        "https://example.com/source/page.html#frag?key1=value&keu2=value",
        "Page Title",
        "file:///output/Page Title.md#frag?key1=value&keu2=value")]
    public void WithPageHeading_NameWithPageHeading(string input, string heading, string expected)
    {
        // Given
        IHost host = CreateDefaultHost();
        Resource resource = new(new Uri(input));
        resource.Metadata.AddMetadata(MarkdownUriTranslater.PageTopHeadingKey, heading);
        var translater = host.Services.GetRequiredService<MarkdownUriTranslater>();

        // When
        Uri result = translater.Translate(resource, resource.Uri);

        // Then
        Assert.Equal(expected, result.ToString());
    }
}
