using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.AngleSharp;

public class AngleSharpWebSource(
    IOptions<SiteExtractorOptions> options,
    IConfiguration? config = null,
    IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(options, config, browsingContext)
{
    public override bool DataExists(Uri uri) => true;

    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(AsAngleSharpUrl(uri)).Result;
}
