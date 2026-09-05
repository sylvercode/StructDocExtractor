using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.AngleSharp;

/// <summary>
/// <see cref="Sources.ISiteSource{TData}"/> implementation that fetches and parses live web pages
/// via AngleSharp, returning the document body as an AngleSharp <see cref="INode"/>.
/// </summary>
/// <remarks>
/// Always reports <see cref="DataExists"/> as <see langword="true"/> because internet resources are
/// assumed reachable. Uses the inherited <see cref="IBrowsingContext"/> to open URLs asynchronously
/// and blocks until the page load completes. Accepts an optional <see cref="ILogger{T}"/> for
/// debug-level logging of each document retrieval.
/// </remarks>
public partial class AngleSharpWebSource(
    IOptions<SiteExtractorOptions> options,
    IConfiguration? config = null,
    IBrowsingContext? browsingContext = null,
    ILogger<AngleSharpWebSource>? logger = null)
    : BaseAngleSharpSiteSource(options, config, browsingContext)
{
    private readonly ILogger<AngleSharpWebSource> _logger = logger ?? NullLogger<AngleSharpWebSource>.Instance;

    /// <inheritdoc/>
    public override bool DataExists(Uri uri) => true;

    /// <inheritdoc/>
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
