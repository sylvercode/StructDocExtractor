namespace Sylvercode.DDBSrcModel.Tests.Extraction;

using System.Collections.Generic;
using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Tests.Stubs;

public class BasicProcessTaskResult(ISrcNode? node = null)
    : BaseExtractor<string, BasicNodeSelectable>.ProcessTaskResult(node ?? new BasicSrcNode())
{
}
