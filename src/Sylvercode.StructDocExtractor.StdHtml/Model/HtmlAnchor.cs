using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlAnchor(string href, string id) : BaseHtmlHref(href, id)
{
    public HtmlAnchor(string href) : this(href, string.Empty)
    {
    }
}
