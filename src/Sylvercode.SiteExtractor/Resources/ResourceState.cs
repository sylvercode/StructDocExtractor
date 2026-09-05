namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Immutable value type tracking the pull state and pullability of a single resource entry.</summary>
/// <remarks>
/// Instances are created via factory methods <see cref="AsPulling"/> and <see cref="AsPulled"/>; each call
/// returns a new <see cref="ResourceState"/> rather than mutating in place.  Attempting to transition
/// an already-pulled or non-pullable resource throws <see cref="InvalidOperationException"/>.
/// </remarks>
public struct ResourceState(bool isPullable)
{
    private readonly ResourcePullState _state = ResourcePullState.Pending;

    private ResourceState(bool isPullable, ResourcePullState state) : this(isPullable)
    {
        _state = state;
    }

    /// <summary>Gets a value indicating whether this resource requires downloading.</summary>
    public readonly bool IsPullable => isPullable;

    /// <summary>Gets a value indicating whether the resource still needs to be pulled (pullable and not yet pulled).</summary>
    public readonly bool IsPullNeeded => isPullable && _state != ResourcePullState.Pulled;

    /// <summary>Gets a value indicating whether the resource is currently being downloaded.</summary>
    public readonly bool IsPulling => isPullable && _state == ResourcePullState.Pulling;

    /// <summary>Gets a value indicating whether the resource has been fully downloaded (or never required pulling).</summary>
    public readonly bool IsPulled => !isPullable || _state == ResourcePullState.Pulled;

    /// <summary>Returns a new <see cref="ResourceState"/> in the <see cref="ResourcePullState.Pulling"/> state.</summary>
    /// <returns>A new <see cref="ResourceState"/> marked as pulling.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resource is not pullable or has already been pulled.</exception>
    public readonly ResourceState AsPulling()
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulling if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulling if it has already been pulled.");

        return new(isPullable, ResourcePullState.Pulling);
    }

    /// <summary>Returns a new <see cref="ResourceState"/> in the <see cref="ResourcePullState.Pulled"/> state.</summary>
    /// <returns>A new <see cref="ResourceState"/> marked as pulled.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resource is not pullable or has already been pulled.</exception>
    public readonly ResourceState AsPulled()
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulled if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulled if it has already been pulled.");

        return new(isPullable, ResourcePullState.Pulled);
    }
}
