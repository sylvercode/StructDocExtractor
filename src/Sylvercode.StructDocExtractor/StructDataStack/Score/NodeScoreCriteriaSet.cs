namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Defines a set of alternative <see cref="NodeScoreCriteria"/> whose highest calculated score is used to rank a discriminator node.</summary>
/// <typeparam name="TDiscriminator">The type of discriminator being scored.</typeparam>
/// <param name="criteria">The alternative criteria to evaluate; the highest score across all alternatives is returned.</param>
/// <remarks>Each <see cref="NodeScoreCriteria"/> represents one alternative path; the first path that produces the highest score wins.</remarks>
public class NodeScoreCriteriaSet<TDiscriminator>(IEnumerable<NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriteria> criteria)
{
    private readonly NodeScoreCriteria[] _criteria = criteria.ToArray();

    /// <summary>Represents a single scoring criterion for one attribute of a discriminator.</summary>
    /// <param name="priority">The relative importance of this criterion; higher values outweigh lower ones when comparing scores.</param>
    /// <param name="matcher">The value matcher applied to the extracted attribute values.</param>
    /// <param name="evaluator">Extracts the relevant attribute values from a discriminator instance.</param>
    public class NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, IEnumerable<string>> evaluator)
    {
        /// <summary>Initializes a new instance of <see cref="NodeScoreCriterion"/> with a single-value evaluator.</summary>
        /// <param name="priority">The relative importance of this criterion.</param>
        /// <param name="matcher">The value matcher applied to the attribute.</param>
        /// <param name="evaluator">Extracts a single attribute string from a discriminator instance.</param>
        public NodeScoreCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, string> evaluator)
            : this(priority, matcher, e => [evaluator.Invoke(e)])
        {

        }

        /// <summary>Gets the relative importance of this criterion; higher values outweigh lower ones.</summary>
        public int Priority => priority;

        /// <summary>Gets the value matcher applied to the discriminator's extracted attribute values.</summary>
        public IValueMatcher Matcher => matcher;

        /// <summary>Evaluates this criterion against a discriminator and returns its match score.</summary>
        /// <param name="node">The discriminator to evaluate.</param>
        /// <returns>A positive score when the matcher finds a match; zero when it does not.</returns>
        public int Match(TDiscriminator node) => matcher.Match(evaluator.Invoke(node));
    }

    /// <summary>Represents a set of <see cref="NodeScoreCriterion"/> instances that must all match for the score to be calculated.</summary>
    /// <param name="subCriteria">The individual criteria that must all pass for this set to produce a non-zero score.</param>
    public class NodeScoreCriteria(IEnumerable<NodeScoreCriterion> subCriteria)
    {
        /// <summary>Gets the individual criteria that must all pass for this set to produce a score.</summary>
        public IEnumerable<NodeScoreCriterion> SubCriteria => subCriteria;

        /// <summary>Calculates the score of the given discriminator against all sub-criteria.</summary>
        /// <param name="node">The discriminator to evaluate.</param>
        /// <returns>A <see cref="NodeScore"/> representing the matched sub-scores, or an empty score if any sub-criterion fails.</returns>
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

    /// <summary>Initializes a new empty <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> that always returns a zero score.</summary>
    public NodeScoreCriteriaSet() : this(Array.Empty<NodeScoreCriteria>())
    { }

    /// <summary>Initializes a new instance of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> with a single criteria alternative.</summary>
    /// <param name="subCriteria">The single <see cref="NodeScoreCriteria"/> alternative to evaluate.</param>
    public NodeScoreCriteriaSet(NodeScoreCriteria subCriteria) : this([subCriteria])
    { }

    /// <summary>Initializes a new instance of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> from a single criterion.</summary>
    /// <param name="subCriterion">The single criterion wrapped into one <see cref="NodeScoreCriteria"/>.</param>
    public NodeScoreCriteriaSet(NodeScoreCriterion subCriterion) : this([subCriterion])
    { }

    /// <summary>Initializes a new instance of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> from an array of criteria forming one alternative.</summary>
    /// <param name="subCriterion">The criteria that together form a single <see cref="NodeScoreCriteria"/> alternative.</param>
    public NodeScoreCriteriaSet(NodeScoreCriterion[] subCriterion) : this([new NodeScoreCriteria(subCriterion)])
    { }

    /// <summary>Initializes a new instance of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> from multiple criterion arrays, each forming a separate alternative.</summary>
    /// <param name="subCriterion">Sequences of criteria where each sequence becomes one <see cref="NodeScoreCriteria"/> alternative.</param>
    public NodeScoreCriteriaSet(IEnumerable<NodeScoreCriterion[]> subCriterion)
            : this(subCriterion.Select(c => new NodeScoreCriteria(c)).ToArray())
    { }

    /// <summary>Calculates the highest score produced by any of the configured criteria alternatives for the given discriminator.</summary>
    /// <param name="node">The discriminator to evaluate.</param>
    /// <returns>The highest <see cref="NodeScore"/> across all alternatives; an empty score if none match.</returns>
    public NodeScore CalculateScore(TDiscriminator node)
    {
        NodeScore score = new();

        foreach (var criterion in _criteria)
        {
            NodeScore newScore = criterion.CalculateScore(node);
            if (newScore.CompareTo(score) > 0)
                score = newScore;
        }

        return score;
    }
}
