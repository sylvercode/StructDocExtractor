namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public interface IStackScoreCalculator<TDiscriminator>
{
    StackedNodesScore Calculate(IStructDataStack<TDiscriminator> stackEntries);
}
