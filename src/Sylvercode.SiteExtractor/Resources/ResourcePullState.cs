namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Defines the possible states a resource can be in during the download/copy cycle.</summary>
public enum ResourcePullState
{
    /// <summary>The resource has been discovered but not yet downloaded.</summary>
    Pending,

    /// <summary>The resource download is currently in progress.</summary>
    Pulling,

    /// <summary>The resource has been successfully downloaded and stored.</summary>
    Pulled
}
