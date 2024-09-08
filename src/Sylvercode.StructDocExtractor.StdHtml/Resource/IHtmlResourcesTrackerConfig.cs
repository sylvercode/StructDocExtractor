using Sylvercode.SiteFetcher.Resource;

namespace Sylvercode.StructDocExtractor.StdHtml.Resource;

public interface IHtmlResourcesTrackerConfig
{
    IResourcePullConfig PullConfig { get; }
    Uri? BaseUri { get; }
}
