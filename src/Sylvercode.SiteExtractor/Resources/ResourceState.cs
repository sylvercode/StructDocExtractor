namespace Sylvercode.SiteExtractor.Resources;

public struct ResourceState(bool isPullable)
{
    private readonly ResourcePullState _state = ResourcePullState.Pending;

    private ResourceState(bool isPullable, ResourcePullState state) : this(isPullable)
    {
        _state = state;
    }

    public readonly bool IsPullable => isPullable;
    public readonly bool IsPullNeeded => isPullable && _state != ResourcePullState.Pulled;
    public readonly bool IsPulling => isPullable && _state == ResourcePullState.Pulling;
    public readonly bool IsPulled => !isPullable || _state == ResourcePullState.Pulled;

    public readonly ResourceState AsPulling()
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulling if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulling if it has already been pulled.");

        return new(isPullable, ResourcePullState.Pulling);
    }

    public readonly ResourceState AsPulled()
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulled if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulled if it has already been pulled.");

        return new(isPullable, ResourcePullState.Pulled);
    }
}
