using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;ul&gt;</c> or <c>&lt;ol&gt;</c> list container whose children are <see cref="HtmlListItem"/> nodes.</summary>
/// <param name="id">The optional node identifier.</param>
public class HtmlList(string id) : BaseStructDocBlockWithAnyParent<HtmlListItem>(id)
{
    /// <summary>Initializes a new instance of <see cref="HtmlList"/> with an empty identifier.</summary>
    public HtmlList() : this(string.Empty)
    {
    }
}
