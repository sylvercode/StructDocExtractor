using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;figure&gt;</c> grouped media block.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlFigure(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlFigure"/> with an empty identifier.</summary>
    public HtmlFigure() : this(string.Empty)
    {
    }
}
