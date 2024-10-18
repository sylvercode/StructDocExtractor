using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlFigure(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlFigure() : this(string.Empty)
    {
    }
}
