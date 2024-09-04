using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests.Mocks;

public class StackScoreCalculatorMock(StackedNodesScore result) : IStackScoreCalculator<BasicNodeDiscriminator>
{
    public StackedNodesScore Calculate(IStructDataStack<BasicNodeDiscriminator> staskEntries)
    {
        return result;
    }
}
