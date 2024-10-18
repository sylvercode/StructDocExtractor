using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableRowData(string id = "") :
    BaseStructDocBlockWithAnyContent<HtmlTableRow>(id),
    IHtmlTableRowElement
{
    public HtmlTableRowData() : this(string.Empty)
    {
    }
}
