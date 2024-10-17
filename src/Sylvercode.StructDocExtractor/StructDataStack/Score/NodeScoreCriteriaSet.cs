namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>
/// Represents differente possible criteria to calculate the score of a discriminator. The score will
/// be the hihest of all criteria.
/// </summary>
/// <typeparam name="TDiscriminator"></typeparam>
/// <param name="criteria"></param>
public readonly struct NodeScoreCriteriaSet<TDiscriminator>(params NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriteria[] criteria)
{
    /// <summary>
    /// Represents a single criterion for an attribut of a discriminator when calculating score.
    /// </summary>
    /// <param name="priority">Priority of the attribut of the dicriminator</param>
    /// <param name="matcher">How to match to the attribut value</param>
    /// <param name="evaluator">How to evaluate the attribute of the dicriminator</param>
    public class NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, IEnumerable<string>> evaluator)
    {
        public NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, string> evaluator)
            : this(priority, matcher, e => [evaluator.Invoke(e)])
        {

        }
        public int Priority => priority;
        public int Match(TDiscriminator node) => matcher.Match(evaluator.Invoke(node));
    }

    /// <summary>
    /// Represents a set of criteria for a single discriminator. All sub criteris must match for 
    /// the score to be calculated.
    /// </summary>
    /// <param name="subCriteria">Criteria to calculate the score</param>
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
