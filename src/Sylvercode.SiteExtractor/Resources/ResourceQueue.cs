namespace Sylvercode.SiteExtractor.Resources;

public class ResourceQueue : IResourceQueue
{
    private readonly Queue<Resource> _queue = new();

    public bool HasQueuedResources => _queue.Count > 0;

    public Resource Dequeue() => _queue.Dequeue();

    public void Enqueue(Resource resource) => _queue.Enqueue(resource);
}
