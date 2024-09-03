using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.DnDBeyond.Model;

public abstract class DDBTOCSectionElement(string text, string href, string id = "") :
    BaseHtmlHref<DDBTOCSection>(text, href, id)
{

}
