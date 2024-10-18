using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlDiv(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlDiv() : this(string.Empty)
    {
    }
}
