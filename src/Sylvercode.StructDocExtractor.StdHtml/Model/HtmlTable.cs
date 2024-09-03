using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlTable(string type = "", string id = "") :
    BaseStructDocBlockWithAnyParent<HtmlTableRow>(id)
{
    public string Type { get; } = type;
}
