using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;tfoot&gt;</c> table footer section, constrained to <see cref="HtmlTableRow"/> children within an <see cref="HtmlTable"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableFooter(string id)
    : BaseStructDocBlock<HtmlTable, HtmlTableRow>(id),
      IHtmlTableElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableFooter"/> with an empty identifier.</summary>
    public HtmlTableFooter() : this(string.Empty)
    {
    }
}
