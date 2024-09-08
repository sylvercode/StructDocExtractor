using Sylvercode.SiteFetcher.Resources;

namespace Sylvercode.SiteFetcher.Tests.Stubs;

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
