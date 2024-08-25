using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public class SrcTable(string type = "", string id = "") :
    BaseSrcBlockWithAnyParent<SrcTableRow>(id)
{
    public string Type { get; } = type;
}
