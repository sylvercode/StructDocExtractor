using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlImg(string src, string id = "") : BaseHtmlHref(src, id)
{
    public string Src => Href;
}
