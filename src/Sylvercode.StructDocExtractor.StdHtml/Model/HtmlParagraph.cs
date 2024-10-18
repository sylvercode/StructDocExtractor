using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlParagraph(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlParagraph() : this(string.Empty)
    {
    }
}
