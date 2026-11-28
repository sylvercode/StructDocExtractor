using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that one primary or continuation processing step returned a result.</summary>
/// <param name="Resource">The tracked resource associated with the result.</param>
/// <param name="Result">The result returned by the processing step.</param>
public sealed record ResourceProcessed(
    Resource Resource,
    IResourceProcessorResult Result) : ExtractionEvent;
