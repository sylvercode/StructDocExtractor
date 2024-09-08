using Sylvercode.SiteFetcher.Resource;

namespace Sylvercode.StructDocExtractor.StdHtml.Resource;

public class HtmlResourcesTrackerConfig(IResourcePullConfig pullConfig, Uri? baseUri)
    : IHtmlResourcesTrackerConfig
{
    public IResourcePullConfig PullConfig { get; } = pullConfig;
    public Uri? BaseUri { get; } = baseUri;
}
