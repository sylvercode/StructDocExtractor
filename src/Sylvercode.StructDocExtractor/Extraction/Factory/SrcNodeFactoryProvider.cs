using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public class SrcNodeFactoryProvider<TExtractionData, TDataSelectable> : ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>
{
    private readonly struct NodeFactoryEntry(ISrcNodeFactory<TExtractionData, TDataSelectable> factory, IStackScoreCalculator<TDataSelectable> scoreCalculator)
    {
        public readonly ISrcNodeFactory<TExtractionData, TDataSelectable> Factory = factory;
        public readonly IStackScoreCalculator<TDataSelectable> ScoreCalculator = scoreCalculator;
    }

    private readonly List<NodeFactoryEntry> factories = [];

    public ISrcNodeFactory<TExtractionData, TDataSelectable>? GetFactoryForStack(IStructDataStack<TDataSelectable> stackEntries)
    {
        StackedNodesScore bestScore = new();
        ISrcNodeFactory<TExtractionData, TDataSelectable>? result = null;
        foreach (var entry in factories)
        {
            StackedNodesScore factoryScore = entry.ScoreCalculator.Calculate(stackEntries);
            if (factoryScore.CompareTo(bestScore) > 0)
            {
                result = entry.Factory;
                bestScore = factoryScore;
            }
        }

        return result;
    }

    public void AddFactory(ISrcNodeFactory<TExtractionData, TDataSelectable> factory, IStackScoreCalculator<TDataSelectable> scoreCalculator)
        => factories.Add(new(factory, scoreCalculator));
}
