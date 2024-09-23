
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.AngleSharp;

public class AngleSharpProxySource(
    IOptions<SiteExtractorOptions> options,
    ISiteSource<string> source,
    IConfiguration? config = null,
    IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(options, config, browsingContext)
{
    public override bool CanGetFrom(Uri uri) => source.CanGetFrom(uri);
    public override bool DataExists(Uri uri) => source.DataExists(uri);
    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(req => req.Content(source.GetData(uri))).Result;
}
