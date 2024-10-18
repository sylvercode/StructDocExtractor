using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableBody(string id)
    : BaseStructDocBlock<HtmlTable, HtmlTableRow>(id),
      IHtmlTableElement
{
    public HtmlTableBody() : this(string.Empty)
    {
    }
}
