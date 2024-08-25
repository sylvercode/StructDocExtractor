using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public abstract class SrcTOCSectionElement(string text, string href, string id = "") :
    BaseSrcHref<SrcTOCSection>(text, href, id)
{

}
