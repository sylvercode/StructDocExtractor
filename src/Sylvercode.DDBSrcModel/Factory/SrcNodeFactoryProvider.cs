using Sylvercode.DDBSrcModel.StructDocStack;
using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Factory;

public class SrcNodeFactoryProvider<N> : ISrcNodeFactoryProvider<N>
{
    private readonly struct NodeFactoryEntry(ISrcNodeFactory factory, IStackScoreCalculator<N> scoreCalculator)
    {
        public readonly ISrcNodeFactory Factory = factory;
        public readonly IStackScoreCalculator<N> ScoreCalculator = scoreCalculator;
    }

    private readonly List<NodeFactoryEntry> factories = [];

    public ISrcNodeFactory? GetFactoryForStack(IStructDataStack<N> staskEntries)
    {
        StackedNodesScore bestScore = new();
        ISrcNodeFactory? result = null;
        foreach (var entry in factories)
        {
            StackedNodesScore factoryScore = entry.ScoreCalculator.Calculate(staskEntries);
            if (factoryScore.CompareTo(bestScore) > 0)
            {
                result = entry.Factory;
                bestScore = factoryScore;
            }
        }

        return result;
    }

    public void AddFactory(ISrcNodeFactory factory, IStackScoreCalculator<N> scoreCalculator)
        => factories.Add(new(factory, scoreCalculator));
}
