using Sylvercode.DDBSrcModel.StructDocStack;
using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Tests.Stubs;

public class StackScoreCalculatorMock(StackedNodesScore result) : IStackScoreCalculator<BasicNodeSelectable>
{
    public StackedNodesScore Calculate(IStructDataStack<BasicNodeSelectable> staskEntries)
    {
        return result;
    }
}
