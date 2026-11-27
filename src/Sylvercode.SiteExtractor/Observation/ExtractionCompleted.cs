using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that the full extraction and all continuations completed successfully.</summary>
/// <param name="Resources">The final read-only resource graph.</param>
public sealed record ExtractionCompleted(
    IReadOnlyResourceRepository Resources) : ExtractionEvent;
