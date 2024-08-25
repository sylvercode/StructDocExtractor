using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public class SrcAside(string type = "", string id = "") :
    BaseSrcBlockWithAnyParentAndContent(id)
{
    public string Type { get; } = type;
}
