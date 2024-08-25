using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Base;

namespace Sylvercode.DDBSrcModel.Tests.Stubs;

public class BasicSrcNode(string id = BasicSrcNode.DefaultId) : BaseSrcNode(id)
{
    public const string DefaultId = nameof(DefaultId);
}
