namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Describes the extraction outcome of a processed resource.</summary>
public enum ResourceExtractionState
{
    /// <summary>The resource has not completed processing.</summary>
    NotProcessed,

    /// <summary>The resource was tracked but skipped because no processing was requested.</summary>
    Skipped,

    /// <summary>Initial extraction prepared metadata and a usable output destination, but output is not yet confirmed.</summary>
    ExtractionPrepared,

    /// <summary>The source returned no data to extract.</summary>
    NoSourceData,

    /// <summary>Processing completed without producing an output.</summary>
    NoOutput,

    /// <summary>Processing completed and produced an output.</summary>
    OutputProduced,

    /// <summary>Processing failed.</summary>
    Failed
}
