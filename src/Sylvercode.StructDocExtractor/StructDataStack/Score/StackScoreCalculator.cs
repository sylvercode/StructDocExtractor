using System.Collections.Immutable;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class StackScoreCalculator<TDiscriminator>(IEnumerable<NodeScoreCriteriaSet<TDiscriminator>> stackEntriesCriteria)
        : IStackScoreCalculator<TDiscriminator>
{
    private readonly ImmutableList<NodeScoreCriteriaSet<TDiscriminator>> _stackEntriesCriteria
            = stackEntriesCriteria.ToImmutableList();

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
            if (!nodeScore.IsEmpty)
            {
                result.Add(stackEntriesIt.Current.Depth, nodeScore);
                hasNextCriterion = criteriaIt.MoveNext();
            }
        }

        return hasNextCriterion ? new StackedNodesScore() : new StackedNodesScore(result);
    }
}
