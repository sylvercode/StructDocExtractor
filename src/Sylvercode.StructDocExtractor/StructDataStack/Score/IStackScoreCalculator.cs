namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Defines the contract for calculating a <see cref="StackedNodesScore"/> from a discriminator stack against a configured set of criteria.</summary>
/// <typeparam name="TDiscriminator">The type of discriminator carried by the stack entries.</typeparam>
public interface IStackScoreCalculator<TDiscriminator>
{
    /// <summary>Calculates the aggregate score of the given stack against the configured criteria.</summary>
    /// <param name="stackEntries">The discriminator stack to evaluate.</param>
    /// <returns>A <see cref="StackedNodesScore"/> representing the combined score; an empty score indicates no match.</returns>
    StackedNodesScore Calculate(IStructDataStack<TDiscriminator> stackEntries);
}
