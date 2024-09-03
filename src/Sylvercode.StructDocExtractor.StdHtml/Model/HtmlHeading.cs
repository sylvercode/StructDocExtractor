using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlHeading(string text, int level, bool inlined = false, string id = "") :
    BaseHtmlTextNode(text, id)
{
    public int Level { get; protected set; } = level;
    public bool Inlined { get; protected set; } = inlined;
}
