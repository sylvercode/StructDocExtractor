using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public interface IReferencerUpdater
{
    void UpdateReferencers(List<IStructDocReferencer> referencers, IReadOnlyResourceRepository resourceRepository);
}
