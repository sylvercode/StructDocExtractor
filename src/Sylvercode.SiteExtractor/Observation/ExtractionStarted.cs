namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that validation for a new extraction has begun.</summary>
public sealed record ExtractionStarted(Uri SeedUri) : ExtractionEvent;
