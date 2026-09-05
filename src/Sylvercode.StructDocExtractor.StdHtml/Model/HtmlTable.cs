using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;table&gt;</c> element, containing <see cref="IHtmlTableElement"/> section nodes.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlTable(string id) :
    BaseStructDocBlockWithAnyParent<IHtmlTableElement>(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlTable"/> with an empty identifier.</summary>
    public HtmlTable() : this(string.Empty)
    {
    }
}
