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
        Uri rebaseUri = base.Translate(resource, uri);
        string path = rebaseUri.IsAbsoluteUri ? rebaseUri.AbsolutePath : uri.ToString();
        if (!resource.Metadata.TryGetStrValue(PageTopHeadingKey, out string? pageTopHeading)
            || string.IsNullOrEmpty(pageTopHeading))
        {
            string pathWithRenameExtention = Path.ChangeExtension(path, MarkdownExtension);
            LogNoPageTopHeading(uri, pathWithRenameExtention);
            return new Uri(pathWithRenameExtention);
        }

        string dirPath = Path.GetDirectoryName(path)?.AsDirPath() ?? string.Empty;
        Uri result = new(new Uri(dirPath), pageTopHeading + MarkdownExtension);
        LogUriTransalted(uri, result);
        return result;
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "No page top heading found for uri {Uri}. Using {PathWithRenameExtention}")]
    private partial void LogNoPageTopHeading(Uri uri, string pathWithRenameExtention);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {Uri} translated to {Result}")]
    private partial void LogUriTransalted(Uri uri, Uri result);
}
