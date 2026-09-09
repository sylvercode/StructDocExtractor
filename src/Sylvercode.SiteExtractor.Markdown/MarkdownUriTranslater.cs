using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.SiteExtractor.Markdown;

/// <summary>URI translater that converts extracted resource URIs to Markdown file paths, using the page's top heading as the file name.</summary>
/// <remarks>
/// Extends <see cref="ResourceUriTranslater"/> by overriding <see cref="Translate"/> to map each resource URI
/// to a <c>.md</c>-suffixed path derived from the resource's <see cref="StdMetadata.PageTopHeadingKey"/> metadata.
/// Invalid filesystem characters in the heading text are replaced with safe substitutes. When no heading is
/// available the original URI path is kept and the <c>.md</c> extension is appended. Depends on
/// <see cref="SiteExtractorOptions"/> for the source base URI used in the initial relative translation.
/// </remarks>
public partial class MarkdownUriTranslater(
    IOptions<SiteExtractorOptions> options,
    ILogger<MarkdownUriTranslater> logger)
    : ResourceUriTranslater(new UriRelativeFromBaseTranslater(options.Value.GetSourceBaseUri()))
{
    private static readonly Uri _tempBaseUri = new("temp://fake.host");

    /// <summary>Gets the file extension appended to all translated Markdown resource paths.</summary>
    public const string MarkdownExtension = ".md";
    private readonly ILogger _logger = logger;

    /// <summary>Translates a resource URI to its Markdown output file path, deriving the file name from the resource's top heading when available.</summary>
    /// <param name="resource">The resource being translated, supplying heading metadata for the file name.</param>
    /// <param name="uri">The source URI to translate.</param>
    /// <returns>A relative URI pointing to the translated <c>.md</c> file path.</returns>
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
            { ':', '.' },
            { '|', '!' },
            { '?', '!' },
            { '#', '+' },
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
