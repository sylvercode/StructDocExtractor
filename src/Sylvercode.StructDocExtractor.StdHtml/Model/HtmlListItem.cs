using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlListItem(string id) : BaseStructDocBlockWithAnyContent<HtmlList>(id)
{
    public HtmlListItem() : this(string.Empty)
    {
    }
}
