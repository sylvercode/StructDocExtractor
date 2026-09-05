using System.Collections.Immutable;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Default <see cref="IStackScoreCalculator{TDiscriminator}"/> that evaluates each stack entry against an ordered list of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> instances.</summary>
/// <typeparam name="TDiscriminator">The type of discriminator carried by the stack entries.</typeparam>
/// <param name="stackEntriesCriteria">The ordered criteria sets; each is matched against the corresponding stack entry from top to bottom.</param>
/// <remarks>All criteria sets must successfully match their corresponding stack entry for a non-empty <see cref="StackedNodesScore"/> to be returned.</remarks>
public class StackScoreCalculator<TDiscriminator>(IEnumerable<NodeScoreCriteriaSet<TDiscriminator>> stackEntriesCriteria)
        : IStackScoreCalculator<TDiscriminator>
{
    private readonly ImmutableList<NodeScoreCriteriaSet<TDiscriminator>> _stackEntriesCriteria
            = stackEntriesCriteria.ToImmutableList();

    /// <inheritdoc/>
    public StackedNodesScore Calculate(IStructDataStack<TDiscriminator> stackEntries)
    {
        if (_stackEntriesCriteria.Count == 0)
            return new StackedNodesScore();

        Dictionary<int, NodeScore> result = [];

        var criteriaIt = _stackEntriesCriteria.GetEnumerator();
        var stackEntriesIt = stackEntries.GetEnumerator();

        var hasNextCriterion = criteriaIt.MoveNext();
        while (hasNextCriterion && stackEntriesIt.MoveNext())
        {
            NodeScore nodeScore = criteriaIt.Current.CalculateScore(stackEntriesIt.Current.NodeDiscriminator);
            if (nodeScore.IsEmpty)
                return new StackedNodesScore();

            result.Add(stackEntriesIt.Current.Depth, nodeScore);
            hasNextCriterion = criteriaIt.MoveNext();
        }

        return hasNextCriterion ? new StackedNodesScore() : new StackedNodesScore(result);
    }
}
