using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlStrong(string id) :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    public HtmlStrong() : this(string.Empty)
    {
    }
}
