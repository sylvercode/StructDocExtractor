using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;p&gt;</c> paragraph element.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlParagraph(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlParagraph"/> with an empty identifier.</summary>
    public HtmlParagraph() : this(string.Empty)
    {
    }
}
