namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Immutable value type tracking processing progress and extraction outcome for one resource.</summary>
/// <param name="isPullable">Whether a processor is available and processing should be scheduled.</param>
/// <remarks>
/// Pull progress answers whether a processor ran. Extraction state answers what that processing produced.
/// Instances are created via factory methods <see cref="AsPulling"/> and <see cref="AsPulled"/>; each call
/// returns a new <see cref="ResourceState"/> rather than mutating in place.  Attempting to transition
/// an already-pulled or non-pullable resource throws <see cref="InvalidOperationException"/>.
/// </remarks>
public struct ResourceState(bool isPullable)
{
    private readonly ResourcePullState _state = ResourcePullState.Pending;

    private readonly ResourceExtractionState _extractionState =
        isPullable
            ? ResourceExtractionState.NotProcessed
            : ResourceExtractionState.Skipped;

    private ResourceState(
        bool isPullable,
        ResourcePullState state,
        ResourceExtractionState extractionState) : this(isPullable)
    {
        _state = state;
        _extractionState = extractionState;
    }

    /// <summary>Gets a value indicating whether this resource requires downloading.</summary>
    public readonly bool IsPullable => isPullable;

    /// <summary>Gets a value indicating whether the resource still needs processing (pullable and not yet completed).</summary>
    public readonly bool IsPullNeeded => isPullable && _state != ResourcePullState.Pulled;

    /// <summary>Gets a value indicating whether the resource is currently being processed.</summary>
    public readonly bool IsPulling => isPullable && _state == ResourcePullState.Pulling;

    /// <summary>Gets a value indicating whether processing is complete, or processing was never required.</summary>
    public readonly bool IsPulled => !isPullable || _state == ResourcePullState.Pulled;

    /// <summary>Gets the current extraction outcome, including skipped or not-yet-processed states.</summary>
    public readonly ResourceExtractionState ExtractionState => _extractionState;

    /// <summary>Returns a new <see cref="ResourceState"/> in the <see cref="ResourcePullState.Pulling"/> state.</summary>
    /// <returns>A new <see cref="ResourceState"/> marked as pulling.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resource is not pullable or has already been pulled.</exception>
    public readonly ResourceState AsPulling()
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulling if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulling if it has already been pulled.");

        return new(
            isPullable,
            ResourcePullState.Pulling,
            _extractionState == ResourceExtractionState.ExtractionPrepared
                ? ResourceExtractionState.ExtractionPrepared
                : ResourceExtractionState.NotProcessed);
    }

    /// <summary>Returns a new <see cref="ResourceState"/> in the <see cref="ResourcePullState.Pulled"/> state.</summary>
    /// <param name="extractionState">The outcome produced by the completed processing step.</param>
    /// <returns>A new <see cref="ResourceState"/> marked as pulled.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resource is not pullable or has already been pulled.</exception>
    public readonly ResourceState AsPulled(
        ResourceExtractionState extractionState = ResourceExtractionState.NoOutput)
    {
        if (!IsPullable)
            throw new InvalidOperationException("Cannot mark as pulled if it doesn't need to be pulled.");

        if (IsPulled)
            throw new InvalidOperationException("Cannot mark as pulled if it has already been pulled.");

        return new(
            isPullable,
            ResourcePullState.Pulled,
            extractionState);
    }

    /// <summary>Returns a new state recording that initial extraction has prepared a usable output destination.</summary>
    /// <returns>A new <see cref="ResourceState"/> in <see cref="ResourceExtractionState.ExtractionPrepared"/>.</returns>
    public readonly ResourceState WithExtractionPrepared()
        => new(isPullable, _state, ResourceExtractionState.ExtractionPrepared);

    /// <summary>Marks the current processing attempt as failed.</summary>
    /// <returns>A new <see cref="ResourceState"/> retaining pull progress and marked as failed.</returns>
    public readonly ResourceState AsFailed()
        => new(
            isPullable,
            _state,
            ResourceExtractionState.Failed);
}
