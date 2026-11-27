using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that the full extraction and all continuations completed successfully.</summary>
public sealed record ExtractionCompleted(
    IReadOnlyResourceRepository Resources) : ExtractionEvent;
