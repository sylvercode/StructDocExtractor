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

    [Fact]
    public void WithPageHeading_NameWithPageHeading()
    {
        // Given
        IHost host = CreateDefaultHost();
        Resource resource = new(new Uri("https://example.com/source/page.html"));
        resource.Metadata.AddMetadata(MarkdownUriTranslater.PageTopHeadingKey, "Page Title");
        var translater = host.Services.GetRequiredService<MarkdownUriTranslater>();

        // When
        Uri result = translater.Translate(resource, resource.Uri);

        // Then
        Assert.Equal("file:///output/Page Title.md", result.ToString());
    }
}
