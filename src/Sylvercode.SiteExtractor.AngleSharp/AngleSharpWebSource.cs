using AngleSharp;
using AngleSharp.Dom;

namespace Sylvercode.SiteExtractor.AngleSharp;

public class AngleSharpWebSource(IConfiguration? config = null, IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(config, browsingContext)
{
    public override bool DataExists(Uri uri) => true;

    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(AsAngleSharpUrl(uri)).Result;
}
