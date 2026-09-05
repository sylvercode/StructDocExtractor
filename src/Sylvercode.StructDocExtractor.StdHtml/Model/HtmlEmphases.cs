using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;em&gt;</c> emphasis element that may contain inline content.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlEmphases(string id) :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlEmphases"/> with an empty identifier.</summary>
    public HtmlEmphases() : this(string.Empty)
    {
    }
}
