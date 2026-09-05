using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Default implementation of <see cref="IStructDocNodeFactoryProvider{TExtractionData, TDataDiscriminator}"/> that selects the best-scoring factory by evaluating stacked-node discriminators against registered criteria.</summary>
/// <typeparam name="TExtractionData">The type of source data element passed to the selected factory.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type whose stack is scored to select a factory.</typeparam>
public class StructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> : IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    private readonly struct NodeFactoryEntry(IStructDocNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
    {
        public readonly IStructDocNodeFactory<TExtractionData, TDataDiscriminator> Factory = factory;
        public readonly IStackScoreCalculator<TDataDiscriminator> ScoreCalculator = scoreCalculator;
    }

    private readonly List<NodeFactoryEntry> factories = [];

    /// <inheritdoc/>
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

    /// <summary>Registers a factory with its associated score calculator for later stack-based resolution.</summary>
    /// <param name="factory">The node factory to register.</param>
    /// <param name="scoreCalculator">The calculator that scores the discriminator stack to determine this factory's priority.</param>
    public void AddFactory(IStructDocNodeFactory<TExtractionData, TDataDiscriminator> factory, IStackScoreCalculator<TDataDiscriminator> scoreCalculator)
        => factories.Add(new(factory, scoreCalculator));
}
