
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;figcaption&gt;</c> caption element, constrained to appear inside an <see cref="HtmlFigure"/>.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlFigCaption(string id) : BaseStructDocBlockWithAnyContent<HtmlFigure>(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlFigCaption"/> with an empty identifier.</summary>
    public HtmlFigCaption() : this(string.Empty)
    {
    }
}
