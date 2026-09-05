
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.AngleSharp;

/// <summary>
/// <see cref="ISiteSource{TData}"/> implementation that parses HTML content retrieved from an
/// underlying string-based <see cref="ISiteSource{TData}"/> through AngleSharp, acting as an
/// AngleSharp adapter over proxy or in-memory sources.
/// </summary>
/// <remarks>
/// Delegates availability checks (<see cref="CanGetFrom"/> and <see cref="DataExists"/>) entirely
/// to the wrapped <paramref name="source"/>, then feeds that source's raw HTML string into AngleSharp's
/// <see cref="IBrowsingContext"/> for DOM parsing. Useful when HTML is pre-fetched or served by a
/// <see cref="MemorySiteSource"/> rather than downloaded live.
/// </remarks>
public class AngleSharpProxySource(
    IOptions<SiteExtractorOptions> options,
    ISiteSource<string> source,
    IConfiguration? config = null,
    IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(options, config, browsingContext)
{
    /// <inheritdoc/>
    public override bool CanGetFrom(Uri uri) => source.CanGetFrom(uri);

    /// <inheritdoc/>
    public override bool DataExists(Uri uri) => source.DataExists(uri);

    /// <inheritdoc/>
    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(req => req.Content(source.GetData(uri))).Result;
}
