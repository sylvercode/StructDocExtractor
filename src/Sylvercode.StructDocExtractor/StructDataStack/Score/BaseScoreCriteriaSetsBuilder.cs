using System.Reflection;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class BaseScoreCriteriaSetsBuilder<TDiscriminator, TBuilder>
    where TBuilder : BaseScoreCriteriaSetsBuilder<TDiscriminator, TBuilder>
{
    private readonly List<NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriterion> _criterionList = [];

    private readonly List<NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriteria> _criteriaList = [];

    private readonly List<NodeScoreCriteriaSet<TDiscriminator>> _criteriaSetList = [];

    public TBuilder Or()
    {
        if (_criterionList.Count == 0)
            return (TBuilder)this;
        _criteriaList.Add(new(_criterionList));
        _criterionList.Clear();
        return (TBuilder)this;
    }

    public TBuilder AndParent()
    {
        Or();
        if (_criteriaList.Count == 0)
            return (TBuilder)this;
        _criteriaSetList.Add(new(_criteriaList.ToArray()));
        _criteriaList.Clear();
        return (TBuilder)this;
    }

    public TBuilder AndParentCriteriaSets(
        IEnumerable<NodeScoreCriteriaSet<TDiscriminator>> criteriaSets)
    {
        AndParent();
        _criteriaSetList.AddRange(criteriaSets);
        return (TBuilder)this;
    }

    public List<NodeScoreCriteriaSet<TDiscriminator>> BuildSets()
    {
        AndParent();
        return _criteriaSetList;
    }

    protected static ValuesMatcherAll NewAllCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
        => new(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());

    protected static ValuesMatcherAny NewAnyCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
        => new(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());

    protected static IEquatable<string> NewEquatable<TEquatable>(string value)
        where TEquatable : IEquatable<string>
    {
        ConstructorInfo ctor = typeof(TEquatable).GetConstructor([typeof(string)])
            ?? throw new InvalidOperationException($"No constructor of one string found for type {typeof(TEquatable).Name}");
        return (IEquatable<string>)ctor.Invoke([value]);
    }

    protected void AddCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, string> evaluator)
        => AddCriterion(priority, matcher, e => [evaluator.Invoke(e)]);

    protected void AddCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, IEnumerable<string>> evaluator)
    {
        if (_criterionList.Any(c => c.Priority == priority))
            throw new InvalidOperationException("Criterion of equal priority already added");

        _criterionList.Add(new NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriterion(priority, matcher, evaluator));
    }
}
