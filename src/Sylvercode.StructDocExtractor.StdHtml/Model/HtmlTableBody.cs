using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;tbody&gt;</c> table body section, constrained to <see cref="HtmlTableRow"/> children within an <see cref="HtmlTable"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableBody(string id)
    : BaseStructDocBlock<HtmlTable, HtmlTableRow>(id),
      IHtmlTableElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableBody"/> with an empty identifier.</summary>
    public HtmlTableBody() : this(string.Empty)
    {
    }
}
