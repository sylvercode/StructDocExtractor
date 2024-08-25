using System.Collections.Immutable;

namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public class StackScoreCalculator<N>(IEnumerable<NodeScoreCriteriaSet<N>> stackEntriesCriteria)
        : IStackScoreCalculator<N>
{
    private readonly ImmutableList<NodeScoreCriteriaSet<N>> _stackEntriesCriteria
            = stackEntriesCriteria.ToImmutableList();

    public StackedNodesScore Calculate(IStructDataStack<N> staskEntries)
    {
        if (_stackEntriesCriteria.Count == 0)
            return new StackedNodesScore();

        Dictionary<int, NodeScore> result = [];

        var criteriaIt = _stackEntriesCriteria.GetEnumerator();
        var stackEntriesIt = staskEntries.GetEnumerator();

        var hasNextCriterion = criteriaIt.MoveNext();
        while (hasNextCriterion && stackEntriesIt.MoveNext())
        {
            NodeScore nodeScore = criteriaIt.Current.CalculateScore(stackEntriesIt.Current.NodeSelectable);
            if (!nodeScore.IsEmpty)
            {
                result.Add(stackEntriesIt.Current.Depth, nodeScore);
                hasNextCriterion = criteriaIt.MoveNext();
            }
        }

        return hasNextCriterion ? new StackedNodesScore() : new StackedNodesScore(result);
    }
}
