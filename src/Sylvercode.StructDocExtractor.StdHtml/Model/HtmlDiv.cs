using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;div&gt;</c> generic block container.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlDiv(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlDiv"/> with an empty identifier.</summary>
    public HtmlDiv() : this(string.Empty)
    {
    }
}
