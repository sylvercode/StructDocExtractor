namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Defines the possible states of the resource processing cycle.</summary>
public enum ResourcePullState
{
    /// <summary>The resource has been discovered but processing has not started.</summary>
    Pending,

    /// <summary>A processor is currently handling the resource.</summary>
    Pulling,

    /// <summary>The resource processing cycle has completed.</summary>
    Pulled
}
