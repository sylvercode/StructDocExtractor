using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class StackScoreCalculatorMock(StackedNodesScore result) : IStackScoreCalculator<BasicNodeDiscriminator>
{
    public StackedNodesScore Calculate(IStructDataStack<BasicNodeDiscriminator> staskEntries)
    {
        return result;
    }
}
