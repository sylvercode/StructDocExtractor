using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;aside&gt;</c> supplemental content block.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlAside(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlAside"/> with an empty identifier.</summary>
    public HtmlAside() : this(string.Empty)
    {
    }
}
