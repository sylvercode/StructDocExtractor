using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Model;

public class SrcHeding(string text, int level, bool inlined = false, string id = "") :
    BaseSrcTextNode(text, id)
{
    public int Level { get; protected set; } = level;
    public bool Inlined { get; protected set; } = inlined;
}
