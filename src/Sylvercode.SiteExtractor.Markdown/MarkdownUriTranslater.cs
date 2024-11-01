using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Markdown;

public partial class MarkdownUriTranslater(
    IOptions<SiteExtractorOptions> options,
    ILogger<MarkdownUriTranslater> logger)
    : ResourceUriTranslater(new UriRelativeFromBaseTranslater(options.Value.GetSourceBaseUri()))
{
    private static readonly Uri _tempBaseUri = new("temp://fake.host");
    public const string MarkdownExtension = ".md";
    private readonly ILogger _logger = logger;

    public override Uri Translate(Resource resource, Uri uri)
    {
        Uri tempRebase = new(_tempBaseUri, base.Translate(resource, uri));
        UriBuilder uriBuilder = new(tempRebase);
        if (!resource.Metadata.TryGetStrValue(StdMetadata.PageTopHeadingKey, out string? pageTopHeading)
            || string.IsNullOrEmpty(pageTopHeading))
        {
            LogNoPageTopHeading(uri);
            uriBuilder.Path = Path.ChangeExtension(uriBuilder.Path, MarkdownExtension);
        }
        else
        {
            pageTopHeading = ReplaceInvalidChars(pageTopHeading);
            string dirPath = Path.GetDirectoryName(uriBuilder.Path)?.AsDirPath() ?? string.Empty;
            uriBuilder.Path = Path.Combine(dirPath, pageTopHeading + MarkdownExtension);
        }
        Uri result = _tempBaseUri.MakeRelativeUri(uriBuilder.Uri);
        LogUriTransalted(uri, result);
        return result;
    }

    static string ReplaceInvalidChars(string str)
    {
        Dictionary<char, char> replacements = new()
        {
            { '*', '+' },
            { '"', '\'' },
            { '\\', '-' },
            { '/', '-' },
            { '<', '{' },
            { '>', '}' },
            { ':', ';' },
            { '|', '!' },
            { '?', '!' },
            { '#', '=' },
            { '^', '\'' },
            { '[', '(' },
            { ']', ')' },
        };

        char[] temp = str.Select(c => replacements.TryGetValue(c, out char replacement) ? replacement : c).ToArray();
        return new(temp);
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
