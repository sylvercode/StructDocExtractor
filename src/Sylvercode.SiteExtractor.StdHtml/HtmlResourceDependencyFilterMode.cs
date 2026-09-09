namespace Sylvercode.SiteExtractor.StdHtml;

/// <summary>Defines the filter mode flags applied to HTML resource dependency links during site extraction.</summary>
/// <remarks>Values can be combined with bitwise OR. <c>None</c> (0) rejects all links for the associated reference type.</remarks>
[Flags]
public enum HtmlResourceDependencyFilterMode
{
    /// <summary>No criteria specified; all dependency links are rejected.</summary>
    None = 0,
    /// <summary>Accepts links whose URI shares the same base as the configured extraction source root.</summary>
    InSourceBase = 1,
    /// <summary>Accepts only links that resolve to an image resource.</summary>
    IsImage = 2,
    /// <summary>Accepts only links that do not resolve to an image resource.</summary>
    IsNotImage = 4,
}
