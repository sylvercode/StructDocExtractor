using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableRow(string id = "") :
    BaseStructDocBlockWithAnyParent<IHtmlTableRowElement>(id),
    IHtmlTableElement
{
    public HtmlTableRow() : this(string.Empty)
    {
    }
}
