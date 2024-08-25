namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public interface IStackScoreCalculator<N>
{
    StackedNodesScore Calculate(IStructDataStack<N> staskEntries);
}
