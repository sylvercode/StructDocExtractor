using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;th&gt;</c> table header cell, constrained to appear inside an <see cref="HtmlTableRow"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableRowHeader(string id = "") :
    BaseStructDocBlockWithAnyContent<HtmlTableRow>(id),
    IHtmlTableRowElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableRowHeader"/> with an empty identifier.</summary>
    public HtmlTableRowHeader() : this(string.Empty)
    {
    }
}
