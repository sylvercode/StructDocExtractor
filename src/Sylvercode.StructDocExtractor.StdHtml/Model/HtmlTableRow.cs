using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableRow(string id = "") :
    BaseStructDocBlock<HtmlTable, HtmlTableRowCell>(id)
{

}
