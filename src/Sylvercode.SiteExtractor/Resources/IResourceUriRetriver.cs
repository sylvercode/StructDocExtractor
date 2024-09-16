
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public interface IResourceUriRetriver
{
    Uri? GetResourceUri(IStructDocNode node);
}
