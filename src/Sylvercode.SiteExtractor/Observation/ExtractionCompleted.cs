using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that the full extraction and all continuations completed successfully.</summary>
/// <param name="Resources">The final read-only resource graph.</param>
public sealed record ExtractionCompleted(
    IReadOnlyResourceRepository Resources) : ExtractionEvent
{
    private List<Action>? _commitActions;

    /// <summary>Registers an action to run after every completion observer succeeds.</summary>
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
