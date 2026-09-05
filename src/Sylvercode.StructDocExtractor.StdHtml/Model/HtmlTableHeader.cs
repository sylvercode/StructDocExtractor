using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;thead&gt;</c> table header section, constrained to <see cref="HtmlTableRow"/> children within an <see cref="HtmlTable"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTableHeader(string id)
    : BaseStructDocBlock<HtmlTable, HtmlTableRow>(id),
      IHtmlTableElement
{
    /// <summary>Initializes a new instance of <see cref="HtmlTableHeader"/> with an empty identifier.</summary>
    public HtmlTableHeader() : this(string.Empty)
    {
    }
}
