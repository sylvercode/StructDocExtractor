namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Contract for processing a single resource by extracting data, copying bytes, or performing similar pipeline work.</summary>
public interface IResourceProcessor
{
    /// <summary>Processes the given resource and returns the outcome of that processing step.</summary>
    /// <param name="resource">The resource to process.</param>
    /// <param name="resourceRepository">The read-only repository of all tracked resources, used to resolve cross-references.</param>
    /// <returns>An <see cref="IResourceProcessorResult"/> describing the outcome; may be unfinished if post-processing is needed.</returns>
    IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository);
}
