
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlFigCaption(string id) : BaseStructDocBlockWithAnyContent<HtmlFigure>(id)
{
    public HtmlFigCaption() : this(string.Empty)
    {
    }
}
