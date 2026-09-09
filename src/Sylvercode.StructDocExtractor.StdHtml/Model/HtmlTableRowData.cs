using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;td&gt;</c> table data cell, constrained to appear inside an <see cref="HtmlTableRow"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableRowData(string id = "") :
    BaseStructDocBlockWithAnyContent<HtmlTableRow>(id),
    IHtmlTableRowElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableRowData"/> with an empty identifier.</summary>
    public HtmlTableRowData() : this(string.Empty)
    {
    }
}
