namespace Sylvercode.SiteFetcher.Resource;

public struct ResourceState(ResourcePullType pullType, bool pulled = false)
{
    public readonly ResourcePullType PullType => pullType;
    public readonly bool IsPullNeeded => pullType is not ResourcePullType.NoPull && !pulled;
}
