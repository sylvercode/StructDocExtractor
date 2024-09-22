namespace Sylvercode.SiteExtractor.Resources;

public enum ResourcePullState
{
    Pending,
    Pulling,
    Pulled
}

public struct ResourceState(ResourcePullType pullType, ResourcePullState state)
{
    public readonly ResourcePullType PullType => pullType;
    public readonly bool IsPullNeeded => pullType is not ResourcePullType.NoPull && state != ResourcePullState.Pulled;
    public readonly bool IsPulling => pullType is not ResourcePullType.NoPull && state == ResourcePullState.Pulling;
}
