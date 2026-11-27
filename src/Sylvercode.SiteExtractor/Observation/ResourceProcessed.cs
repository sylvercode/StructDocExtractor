using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Indicates that one primary or continuation processing step returned a result.</summary>
public sealed record ResourceProcessed(
    Resource Resource,
    IResourceProcessorResult Result) : ExtractionEvent;
