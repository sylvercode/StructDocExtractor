using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;tr&gt;</c> table row, containing <see cref="IHtmlTableRowElement"/> cell nodes.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableRow(string id = "") :
    BaseStructDocBlockWithAnyParent<IHtmlTableRowElement>(id),
    IHtmlTableElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableRow"/> with an empty identifier.</summary>
    public HtmlTableRow() : this(string.Empty)
    {
    }
}
