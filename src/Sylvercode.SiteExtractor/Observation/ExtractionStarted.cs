namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that validation for a new extraction has begun.</summary>
/// <param name="SeedUri">The root URI passed to the extraction.</param>
public sealed record ExtractionStarted(Uri SeedUri) : ExtractionEvent;
