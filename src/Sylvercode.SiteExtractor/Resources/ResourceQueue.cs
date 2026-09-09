using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Resources;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Thread-safe FIFO queue pairing each pending resource with the processor assigned to handle it.</summary>
public class ResourceQueue
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Queue<KeyValuePair<Resource, IResourceProcessor>> _queue = new();

    /// <summary>Gets a value indicating whether there are resources waiting to be processed.</summary>
    public bool HasQueuedResources => _queue.Count > 0;

    /// <summary>Removes and returns the next resource/processor pair from the front of the queue.</summary>
    /// <returns>The next <see cref="Resource"/> and its associated <see cref="IResourceProcessor"/>.</returns>
    public KeyValuePair<Resource, IResourceProcessor> Dequeue() => _queue.Dequeue();

    /// <summary>Adds a resource and its assigned processor to the back of the queue.</summary>
    /// <param name="resource">The resource to be processed.</param>
    /// <param name="processor">The processor responsible for handling the resource.</param>
    public void Enqueue(Resource resource, IResourceProcessor processor)
        => _queue.Enqueue(new(resource, processor));
}
