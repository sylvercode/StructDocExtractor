using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public class SrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> : ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    private readonly struct NodeFactoryEntry(ISrcNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
    {
        public readonly ISrcNodeFactory<TExtractionData, TDataDiscriminator> Factory = factory;
        public readonly IStackScoreCalculator<TDataDiscriminator> ScoreCalculator = scoreCalculator;
    }

    private readonly List<NodeFactoryEntry> factories = [];

    public ISrcNodeFactory<TExtractionData, TDataDiscriminator>? GetFactoryForStack(IStructDataStack<TDataDiscriminator> stackEntries)
    {
        StackedNodesScore bestScore = new();
        ISrcNodeFactory<TExtractionData, TDataDiscriminator>? result = null;
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

    public void AddFactory(ISrcNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
        => factories.Add(new(factory, scoreCalculator));
}
