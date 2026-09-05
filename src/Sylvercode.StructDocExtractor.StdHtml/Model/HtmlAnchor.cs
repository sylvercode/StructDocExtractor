using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;a&gt;</c> anchor element, carrying an href URI and optional inline content.</summary>
/// <param name="href">The URI of the hyperlink.</param>
/// <param name="id">The optional node identifier.</param>
public class HtmlAnchor(string href, string id) :
    BaseHtmlHref<IStructDocNodeHolder>(string.Empty, href, id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlAnchor"/> with the specified href and an empty identifier.</summary>
    /// <param name="href">The URI of the hyperlink.</param>
    public HtmlAnchor(string href) : this(href, string.Empty)
    {
    }
}
