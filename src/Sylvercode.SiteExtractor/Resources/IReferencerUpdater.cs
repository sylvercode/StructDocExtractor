using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public interface IReferencerUpdater
{
    void UpdateReferencers(Resource referencerResource, List<IStructDocReferencer> referencers, IReadOnlyResourceRepository resourceRepository);
}
