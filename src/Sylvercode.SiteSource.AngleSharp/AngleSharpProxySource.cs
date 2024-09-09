
using AngleSharp;
using AngleSharp.Dom;

namespace Sylvercode.SiteSource.AngleSharp;

public class AngleSharpProxySource(ISiteSource<string> source, IConfiguration? config = null, IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(config, browsingContext)
{
    public override bool CanGetFrom(Uri uri) => source.CanGetFrom(uri);
    public override bool DataExists(Uri uri) => source.DataExists(uri);
    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(req => req.Content(source.GetData(uri))).Result;
}
