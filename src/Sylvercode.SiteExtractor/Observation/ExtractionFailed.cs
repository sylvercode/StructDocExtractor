using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that an extraction operation failed and will propagate its exception.</summary>
/// <param name="Exception">The original exception that stopped extraction.</param>
/// <param name="Resource">The resource involved, or <see langword="null"/> for a global failure.</param>
/// <param name="Operation">The operation that was running when the failure occurred.</param>
public sealed record ExtractionFailed(
    Exception Exception,
    Resource? Resource,
    string Operation) : ExtractionEvent;
