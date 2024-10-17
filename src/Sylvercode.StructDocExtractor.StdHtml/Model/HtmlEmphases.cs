using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlEmphases(string id) :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlEmphases() : this(string.Empty)
    {
    }
}
