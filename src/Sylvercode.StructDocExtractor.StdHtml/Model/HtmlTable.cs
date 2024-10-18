using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTable(string id) :
    BaseStructDocBlockWithAnyParent<IHtmlTableElement>(id)
{
    public HtmlTable() : this(string.Empty)
    {
    }
}
