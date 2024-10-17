using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlHeading(int level, string id = "") :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    public int Level { get; protected set; } = level;
}
