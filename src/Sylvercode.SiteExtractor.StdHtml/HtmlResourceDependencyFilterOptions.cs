using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.StdHtml;

/// <summary>Configuration options that control how the HTML resource dependency filter classifies external and embedded resource links.</summary>
public class HtmlResourceDependencyFilterOptions
{
    /// <summary>Gets or sets the filter mode applied to external resource links (e.g., hyperlinks to other pages).</summary>
    public HtmlResourceDependencyFilterMode ExternalFilterMode { get; set; } =
        HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsNotImage;

    /// <summary>Gets or sets the filter mode applied to embedded resource links (e.g., image <c>src</c> attributes).</summary>
    public HtmlResourceDependencyFilterMode EmbededFilterMode { get; set; } =
        HtmlResourceDependencyFilterMode.IsImage;

    /// <summary>Returns the filter mode configured for the given reference type.</summary>
    /// <param name="refType">The type of reference (external link or embedded asset) being evaluated.</param>
    /// <returns>The <see cref="HtmlResourceDependencyFilterMode"/> that applies to <paramref name="refType"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="refType"/> is not a recognised value.</exception>
    public HtmlResourceDependencyFilterMode GetFilterMode(IStructDocReferencer.ReferenceType refType)
    {
        return refType switch
        {
            IStructDocReferencer.ReferenceType.External => ExternalFilterMode,
            IStructDocReferencer.ReferenceType.Embeded => EmbededFilterMode,
            _ => throw new ArgumentOutOfRangeException(nameof(refType)),
        };
    }
}
