using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that an extraction operation failed and will propagate its exception.</summary>
public sealed record ExtractionFailed(
    Exception Exception,
    Resource? Resource,
    string Operation) : ExtractionEvent;
