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
    private readonly ILogger _logger = logger;

    public override Uri Translate(Resource resource, Uri uri)
    {
        Uri rebaseUri = base.Translate(resource, uri);
        if (!resource.Metadata.TryGetStrValue("page-top-heading", out string? pageTopHeading)
            || string.IsNullOrEmpty(pageTopHeading))
        {
            LogNoPageTopHeading(uri);
            return rebaseUri;
        }

        string path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.ToString();
        string dirPath = Path.GetDirectoryName(path) ?? string.Empty;
        Uri result = new(new Uri(dirPath), pageTopHeading);
        LogUriTransalted(uri, result);
        return result;
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
