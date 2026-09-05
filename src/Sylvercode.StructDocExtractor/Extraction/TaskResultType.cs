namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the possible outcome classifications for a processed extraction task.</summary>
public enum TaskResultType
{
    /// <summary>Indicates the task completed successfully and produced a structural node.</summary>
    Success,
    /// <summary>Indicates the task completed but encountered a non-fatal issue worth noting.</summary>
    Warning,
    /// <summary>Indicates the task failed due to an error during extraction.</summary>
    Error,
    /// <summary>Indicates the task was intentionally skipped and produced no output.</summary>
    Skipped,
}
