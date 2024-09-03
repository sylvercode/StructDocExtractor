using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlStrongNode(string text, string id = "") :
    BaseHtmlTextNode(text, id)
{

}
