using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.AngleSharp;

public partial class AngleSharpWebSource(
    IOptions<SiteExtractorOptions> options,
    IConfiguration? config = null,
    IBrowsingContext? browsingContext = null,
    ILogger<AngleSharpWebSource>? logger = null)
    : BaseAngleSharpSiteSource(options, config, browsingContext)
{
    private readonly ILogger<AngleSharpWebSource> _logger = logger ?? NullLogger<AngleSharpWebSource>.Instance;

    public override bool DataExists(Uri uri) => true;

    protected override IDocument GetDocument(Uri uri)
    {
        LogGetDocument(uri);
        return BrowsingContext.OpenAsync(AsAngleSharpUrl(uri)).Result;
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Get document from {Uri}")]
    private partial void LogGetDocument(Uri uri);
}
