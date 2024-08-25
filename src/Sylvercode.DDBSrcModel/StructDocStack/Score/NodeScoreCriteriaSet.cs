namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public readonly struct NodeScoreCriteriaSet<N>(params NodeScoreCriteriaSet<N>.NodeScoreCriteria[] criteria)
{
    public class NodeScoreCriterion(int priority, IValueMatcher matcher, Func<N, IEnumerable<string>> evaluator)
    {
        public NodeScoreCriterion(int priority, IValueMatcher matcher, Func<N, string> evaluator)
            : this(priority, matcher, e => [evaluator.Invoke(e)])
        {

        }
        public int Priority => priority;
        public int Match(N node) => matcher.Match(evaluator.Invoke(node));
    }

    public class NodeScoreCriteria(params NodeScoreCriterion[] subCriteria)
    {
        public NodeScore CalculateScore(N node)
        {
            List<NodeScore.SubScore> result = [];
            foreach (var subCriterion in subCriteria)
            {
                int match = subCriterion.Match(node);
                if (match == 0)
                    return new();

                result.Add(new(subCriterion.Priority, match));
            }

            return new(result);
        }
    }

    public NodeScoreCriteriaSet(params NodeScoreCriterion[] subCriterion) : this(new NodeScoreCriteria(subCriterion))
    { }

    public NodeScoreCriteriaSet(IEnumerable<NodeScoreCriterion[]> subCriterion)
            : this(subCriterion.Select(c => new NodeScoreCriteria(c)).ToArray())
    { }

    public NodeScore CalculateScore(N node)
    {
        if (criteria is null)
            return new();

        NodeScore score = new();

        foreach (var criterion in criteria)
        {
            NodeScore newScore = criterion.CalculateScore(node);
            if (newScore.CompareTo(score) > 0)
                score = newScore;
        }

        return score;
    }
}
