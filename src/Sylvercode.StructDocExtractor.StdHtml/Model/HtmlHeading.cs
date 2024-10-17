using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlHeading(int level, bool inlined = false, string id = "") :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    public int Level { get; protected set; } = level;
    public bool Inlined { get; protected set; } = inlined;
}
