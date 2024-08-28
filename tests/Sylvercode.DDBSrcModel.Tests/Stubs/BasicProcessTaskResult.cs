namespace Sylvercode.DDBSrcModel.Tests.Extraction;

using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Tests.Stubs;

public class BasicProcessTaskResult(ISrcNode? node = null)
    : ProcessTaskResult<string, BasicNodeSelectable>(node)
{
}
