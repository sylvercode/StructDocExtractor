using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Contract for updating embedded URIs inside a resource's serialized content after extraction.</summary>
public interface IReferencerUpdater
{
    /// <summary>Rewrites the URI reference in each <see cref="IStructDocReferencer"/> to its translated output path.</summary>
    /// <param name="referencerResource">The resource whose serialized content contains the references being updated.</param>
    /// <param name="referencers">The list of reference nodes whose URIs should be rewritten.</param>
    /// <param name="resourceRepository">The repository used to look up the translated URI for each referenced resource.</param>
    void UpdateReferencers(Resource referencerResource, List<IStructDocReferencer> referencers, IReadOnlyResourceRepository resourceRepository);
}
