using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableFooter(string id)
    : BaseStructDocBlock<HtmlTable, HtmlTableRow>(id),
      IHtmlTableElement
{
    public HtmlTableFooter() : this(string.Empty)
    {
    }
}
