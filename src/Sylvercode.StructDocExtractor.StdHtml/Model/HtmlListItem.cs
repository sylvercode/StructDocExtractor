using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;li&gt;</c> list item, constrained to appear inside an <see cref="HtmlList"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlListItem(string id) : BaseStructDocBlockWithAnyContent<HtmlList>(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlListItem"/> with an empty identifier.</summary>
    public HtmlListItem() : this(string.Empty)
    {
    }
}
