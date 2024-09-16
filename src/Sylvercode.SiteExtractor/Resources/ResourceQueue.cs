using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceQueue
{
    private readonly Queue<KeyValuePair<Resource, IResourceProcessor>> _queue = new();

    public bool HasQueuedResources => _queue.Count > 0;

    public KeyValuePair<Resource, IResourceProcessor> Dequeue() => _queue.Dequeue();

    public void Enqueue(Resource resource, IResourceProcessor processor)
        => _queue.Enqueue(new(resource, processor));
}
