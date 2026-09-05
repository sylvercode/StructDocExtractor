namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Contract for processors that parse a resource document and extract structured data from it.</summary>
/// <typeparam name="TExtractionData">The type of raw data provided by the site source for extraction.</typeparam>
public interface IResourceDataExtractor<TExtractionData> : IResourceProcessor
{
    /// <summary>Extracts structured data from the given resource and returns the processing outcome.</summary>
    /// <param name="resource">The resource whose document content should be extracted.</param>
    /// <param name="resourceRepository">The repository of all tracked resources used to resolve cross-references.</param>
    /// <returns>An <see cref="IResourceProcessorResult"/> that may carry extracted nodes and discovered dependency URIs.</returns>
    IResourceProcessorResult Extract(Resource resource, IReadOnlyResourceRepository resourceRepository);
}
