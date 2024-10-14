using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Markdown;

public partial class MarkdownUriTranslater(
    IOptions<SiteExtractorOptions> options,
    ILogger<MarkdownUriTranslater> logger)
    : ResourceUriTranslater(new UriBaseTranslater(options.Value.GetSourceBaseUri(), options.Value.GetOutputUri()))
{
    public const string PageTopHeadingKey = "page-top-heading";
    public const string MarkdownExtension = ".md";
    private readonly ILogger _logger = logger;

    public override Uri Translate(Resource resource, Uri uri)
    {
        UriBuilder uriBuilder = new(base.Translate(resource, uri));
        if (!resource.Metadata.TryGetStrValue(PageTopHeadingKey, out string? pageTopHeading)
            || string.IsNullOrEmpty(pageTopHeading))
        {
            LogNoPageTopHeading(uri);
            uriBuilder.Path = Path.ChangeExtension(uriBuilder.Path, MarkdownExtension);
        }
        else
        {
            string dirPath = Path.GetDirectoryName(uriBuilder.Path)?.AsDirPath() ?? string.Empty;
            uriBuilder.Path = Path.Combine(dirPath, pageTopHeading + MarkdownExtension);
        }

        LogUriTransalted(uri, uriBuilder.Uri);
        return uriBuilder.Uri;
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "No page top heading found for uri {Uri}")]
    private partial void LogNoPageTopHeading(Uri uri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {Uri} translated to {Result}")]
    private partial void LogUriTransalted(Uri uri, Uri result);
}
