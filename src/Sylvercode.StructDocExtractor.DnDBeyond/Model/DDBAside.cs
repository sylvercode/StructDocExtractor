using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.DnDBeyond.Model;

public class DDBAside(string type = "", string id = "") :
    BaseStructDocBlockWithAnyParentAndContent(id)
{
    public string Type { get; } = type;
}
