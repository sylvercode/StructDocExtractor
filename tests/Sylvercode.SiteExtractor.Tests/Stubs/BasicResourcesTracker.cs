using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

public class BasicResourcesTracker(ResourceDictionary resourceDictionary,
                                   IResourcePullConfig config) : BaseResourcesTracker(resourceDictionary, config)
{
    protected override Uri? GetUri(IStructDocNode node)
    {
        if (node is UriNode uri)
            return uri.Uri;
        return null;
    }
}
