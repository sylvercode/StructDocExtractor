using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Coordinates resource discovery, deduplication, and processor assignment by combining a <see cref="ResourceRepository"/> with a <see cref="ResourceQueue"/>.</summary>
/// <remarks>
/// When a new URI is added via <see cref="AddResource(Uri)"/>, the tracker resolves the appropriate
/// <see cref="IResourceProcessor"/> from the injected <see cref="IResourceProcessorProvider"/> and enqueues
/// the resource for processing if a processor is found.  URIs that do not match any processor are still
/// tracked in the repository as non-pullable entries; duplicate URIs are silently ignored and return
/// <see langword="false"/>.
/// </remarks>
public class ResourcesTracker(IResourceProcessorProvider processorProvider)
{
    private readonly ResourceRepository _resourceRepository = [];

    /// <summary>Gets the read-only view of all resources discovered so far.</summary>
    public IReadOnlyResourceRepository Resources => _resourceRepository;

    /// <summary>Gets the queue of resources pending extraction or download.</summary>
    public ResourceQueue ResourceQueue { get; } = new();

    /// <summary>Adds the resource at <paramref name="uri"/> to the tracker, enqueueing it for processing if a matching processor exists.</summary>
    /// <param name="uri">The URI of the resource to track.</param>
    /// <returns><see langword="true"/> if the resource was newly added; <see langword="false"/> if already tracked.</returns>
    public bool AddResource(Uri uri) => AddResource(uri, out _);

    /// <summary>Adds the resource at <paramref name="uri"/> to the tracker and reports whether a processor was found.</summary>
    /// <param name="uri">The URI of the resource to track.</param>
    /// <param name="hasResourceProcessor">Set to <see langword="true"/> if a matching processor was resolved; otherwise <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the resource was newly added; <see langword="false"/> if already tracked.</returns>
    public bool AddResource(Uri uri, out bool hasResourceProcessor)
    {
        IResourceProcessor? resourceProcessor = processorProvider.GetProcessor(uri);
        hasResourceProcessor = resourceProcessor is not null;

        var (resource, isNew) = _resourceRepository.Add(uri, resourceProcessor is not null);
        if (resourceProcessor is not null && isNew)
            ResourceQueue.Enqueue(resource, resourceProcessor);

        return isNew;
    }
}
