using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public class StructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> : IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    private readonly struct NodeFactoryEntry(IStructDocNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
    {
        public readonly IStructDocNodeFactory<TExtractionData, TDataDiscriminator> Factory = factory;
        public readonly IStackScoreCalculator<TDataDiscriminator> ScoreCalculator = scoreCalculator;
    }

    private readonly List<NodeFactoryEntry> factories = [];

    public IStructDocNodeFactory<TExtractionData, TDataDiscriminator>? GetFactoryForStack(IStructDataStack<TDataDiscriminator> stackEntries)
    {
        StackedNodesScore bestScore = new();
        IStructDocNodeFactory<TExtractionData, TDataDiscriminator>? result = null;
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

    public void AddFactory(IStructDocNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
        => factories.Add(new(factory, scoreCalculator));
}
