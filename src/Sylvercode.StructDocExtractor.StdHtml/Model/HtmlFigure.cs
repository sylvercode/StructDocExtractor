using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlFigure(string href, string text = "", string id = "") :
    BaseHtmlHref(href, text, id)
{
}
