using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

public class BasicResourcesUriRetriver() : IResourceUriRetriver
{
    public Uri? GetResourceUri(IStructDocNode node)
    {
        if (node is UriNode uri)
            return uri.Uri;
        return null;
    }
}
