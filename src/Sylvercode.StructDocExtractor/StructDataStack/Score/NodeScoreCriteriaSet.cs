namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public readonly struct NodeScoreCriteriaSet<TDiscriminator>(params NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriteria[] criteria)
{
    public class NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, IEnumerable<string>> evaluator)
    {
        public NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, string> evaluator)
            : this(priority, matcher, e => [evaluator.Invoke(e)])
        {

        }
        public int Priority => priority;
        public int Match(TDiscriminator node) => matcher.Match(evaluator.Invoke(node));
    }

    public class NodeScoreCriteria(IEnumerable<NodeScoreCriterion> subCriteria)
    {
        public NodeScore CalculateScore(TDiscriminator node)
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

    public NodeScore CalculateScore(TDiscriminator node)
    {
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
