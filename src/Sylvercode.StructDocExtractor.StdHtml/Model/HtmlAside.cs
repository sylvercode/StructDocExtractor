using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlAside(string id) : BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlAside() : this(string.Empty)
    {
    }
}
