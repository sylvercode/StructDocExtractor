using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

public class BasicResourcesTracker(ResourceDictionary resourceDictionary,
                                   IResourcePullConfig config) : BaseResourcesTracker(resourceDictionary, config)
{
    protected override Uri? GetUri(object sender)
    {
        if (sender is Uri uri)
            return uri;
        return null;
    }
}
