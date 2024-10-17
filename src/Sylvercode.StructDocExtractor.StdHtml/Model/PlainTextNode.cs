using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class PlainTextNode(string text) :
    BaseHtmlTextNode(text, "")
{

}
