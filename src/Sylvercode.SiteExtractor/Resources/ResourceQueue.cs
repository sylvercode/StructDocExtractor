using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Resources;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class ResourceQueue
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    private readonly Queue<KeyValuePair<Resource, IResourceProcessor>> _queue = new();

    public bool HasQueuedResources => _queue.Count > 0;

    public KeyValuePair<Resource, IResourceProcessor> Dequeue() => _queue.Dequeue();

    public void Enqueue(Resource resource, IResourceProcessor processor)
        => _queue.Enqueue(new(resource, processor));
}
