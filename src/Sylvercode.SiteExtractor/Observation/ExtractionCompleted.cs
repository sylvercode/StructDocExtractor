using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that the full extraction and all continuations completed successfully.</summary>
/// <param name="Resources">The final read-only resource graph.</param>
/// <remarks>
/// Observers may prepare or validate their completion work during notification. Registered commit
/// actions run only after every observer accepts this event. A framework-owned finalization may run
/// after ordinary commits; irreversible effects outside that finalization are not transactional and
/// remain the responsibility of the observer that registered them, including any compensation or
/// rollback. No multi-observer transaction or global rollback is provided.
/// </remarks>
public sealed record ExtractionCompleted(
    IReadOnlyResourceRepository Resources) : ExtractionEvent
{
    private List<Action>? _commitActions;

    /// <summary>Registers a potentially fallible ordinary commit to run after every observer succeeds.</summary>
    /// <remarks>
    /// Register only work that is complete or independently compensatable. Commits are executed in
    /// registration order and are not rolled back if a later commit or finalization fails.
    /// </remarks>
    /// <param name="commitAction">The side effect to commit.</param>
    public void AddCommitAction(Action commitAction)
        => (_commitActions ??= []).Add(commitAction);

    /// <summary>Executes all actions registered by completion observers.</summary>
    public void Commit()
    {
        foreach (Action commitAction in _commitActions ?? [])
            commitAction();
    }
}
