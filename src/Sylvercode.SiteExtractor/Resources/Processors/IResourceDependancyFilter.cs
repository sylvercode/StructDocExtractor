using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public interface IResourceDependancyFilter
{
    bool IsAccepted(IStructDocReferencer referencer);
}
