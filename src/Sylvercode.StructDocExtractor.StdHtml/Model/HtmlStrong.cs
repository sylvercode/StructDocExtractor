using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;strong&gt;</c> bold element that may contain inline content.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlStrong(string id) :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlStrong"/> with an empty identifier.</summary>
    public HtmlStrong() : this(string.Empty)
    {
    }
}
