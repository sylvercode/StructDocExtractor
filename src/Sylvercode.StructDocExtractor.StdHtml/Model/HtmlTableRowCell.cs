using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTableRowCell(string id = "") :
    BaseStructDocBlockWithAnyContent<HtmlTableRow>(id)
{

}
