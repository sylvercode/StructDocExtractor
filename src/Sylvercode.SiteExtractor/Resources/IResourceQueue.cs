namespace Sylvercode.SiteExtractor.Resources;

public interface IResourceQueue
{
    public void Enqueue(Resource resource);
    public Resource Dequeue();
    public bool HasQueuedResources { get; }
}
