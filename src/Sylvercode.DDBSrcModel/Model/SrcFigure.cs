using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public class SrcFigure(string href, string text = "", string id = "") :
    BaseSrcHref(href, text, id)
{
}
