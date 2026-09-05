using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML heading element (<c>h1</c>–<c>h6</c>) with its heading level.</summary>
/// <param name="level">The heading level (1–6) corresponding to h1–h6.</param>
/// <param name="id">The optional node identifier.</param>
public class HtmlHeading(int level, string id = "") :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    /// <summary>Gets the heading level (1–6) corresponding to <c>h1</c>–<c>h6</c>.</summary>
    public int Level { get; protected set; } = level;
}
