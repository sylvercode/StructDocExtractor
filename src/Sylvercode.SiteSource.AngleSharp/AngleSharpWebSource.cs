
using AngleSharp;
using AngleSharp.Dom;

namespace Sylvercode.SiteSource.AngleSharp;

public class AngleSharpWebSource(IConfiguration? config = null, IBrowsingContext? browsingContext = null)
    : BaseAngleSharpSiteSource(config, browsingContext)
{
    private readonly HttpClient _httpClient = new();

    public override bool DataExists(Uri uri) =>
        _httpClient.GetAsync(uri).Result.EnsureSuccessStatusCode().IsSuccessStatusCode;

    protected override IDocument GetDocument(Uri uri)
        => BrowsingContext.OpenAsync(AsAngleSharpUrl(uri)).Result;
}
