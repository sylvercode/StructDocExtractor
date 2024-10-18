using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableRowHeader(string id = "") :
    BaseStructDocBlockWithAnyContent<HtmlTableRow>(id),
    IHtmlTableRowElement
{
    public HtmlTableRowHeader() : this(string.Empty)
    {
    }
}
