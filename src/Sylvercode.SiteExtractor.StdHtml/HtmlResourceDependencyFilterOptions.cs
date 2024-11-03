using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.StdHtml;

public class HtmlResourceDependencyFilterOptions
{
    public HtmlResourceDependencyFilterMode ExternalFilterMode { get; set; } =
        HtmlResourceDependencyFilterMode.InSourceBase | HtmlResourceDependencyFilterMode.IsNotImage;

    public HtmlResourceDependencyFilterMode EmbededFilterMode { get; set; } =
        HtmlResourceDependencyFilterMode.IsImage;

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
