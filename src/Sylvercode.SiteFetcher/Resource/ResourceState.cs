namespace Sylvercode.SiteFetcher.Resource;

public class ResourceState(ResourcePullType pullType, bool pulled = false)
{
    public ResourcePullType PullType => pullType;
    public bool IsPullNeeded { get; } = pullType is not ResourcePullType.NoPull && !pulled;
}
