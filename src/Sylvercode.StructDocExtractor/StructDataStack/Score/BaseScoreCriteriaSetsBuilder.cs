using System.Reflection;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Base class for building collections of <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> through a fluent API, supporting multi-criterion and multi-parent scoring rules.</summary>
/// <typeparam name="TDiscriminator">The type of discriminator being scored.</typeparam>
/// <typeparam name="TBuilder">The concrete subclass type, enabling fluent method chaining via the curiously recurring template pattern.</typeparam>
/// <remarks>
/// Subclasses call <see cref="AddCriterion(int, IValueMatcher, Func{TDiscriminator, string})"/> to register scoring criteria,
/// then use <see cref="Or"/>, <see cref="AndParent"/>, and <see cref="BuildSets"/> to compose the final criteria set list.
/// </remarks>
public class BaseScoreCriteriaSetsBuilder<TDiscriminator, TBuilder>
    where TBuilder : BaseScoreCriteriaSetsBuilder<TDiscriminator, TBuilder>
{
    private List<NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriterion> _criterionList = [];

    private List<NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriteria> _criteriaList = [];

    private List<NodeScoreCriteriaSet<TDiscriminator>> _criteriaSetList = [];

    /// <summary>Seals the current criterion list into a <see cref="NodeScoreCriteriaSet{TDiscriminator}.NodeScoreCriteria"/> OR alternative and begins a new one.</summary>
    /// <returns>This builder instance for chaining.</returns>
    public TBuilder Or()
    {
        if (_criterionList.Count == 0)
            return (TBuilder)this;
        _criteriaList.Add(new(_criterionList));
        _criterionList = [];
        return (TBuilder)this;
    }

    /// <summary>Seals the current criteria into a complete <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> and begins collecting criteria for the next ancestor stack entry.</summary>
    /// <returns>This builder instance for chaining.</returns>
    public TBuilder AndParent()
    {
        Or();
        if (_criteriaList.Count == 0)
            return (TBuilder)this;
        _criteriaSetList.Add(new(_criteriaList.ToArray()));
        _criteriaList = [];
        return (TBuilder)this;
    }

    /// <summary>Seals the current criteria and appends externally built <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> instances for subsequent ancestor levels.</summary>
    /// <param name="criteriaSets">The pre-built criteria sets to append for deeper ancestor matching.</param>
    /// <returns>This builder instance for chaining.</returns>
    public TBuilder AndParentCriteriaSets(
        IEnumerable<NodeScoreCriteriaSet<TDiscriminator>> criteriaSets)
    {
        AndParent();
        _criteriaSetList.AddRange(criteriaSets);
        return (TBuilder)this;
    }

    /// <summary>Finalizes the builder and returns all accumulated <see cref="NodeScoreCriteriaSet{TDiscriminator}"/> instances, resetting the builder state.</summary>
    /// <returns>The list of built criteria sets, ordered from innermost to outermost ancestor.</returns>
    public List<NodeScoreCriteriaSet<TDiscriminator>> BuildSets()
    {
        AndParent();
        (List<NodeScoreCriteriaSet<TDiscriminator>> result, _criteriaSetList) = (_criteriaSetList, []);
        return result;
    }

    /// <summary>Creates a <see cref="ValuesMatcherAll"/> whose matchers are constructed from the given string values using reflection on <typeparamref name="TEquatable"/>.</summary>
    /// <typeparam name="TEquatable">The <see cref="IEquatable{String}"/> type to instantiate for each value; must have a single-string constructor.</typeparam>
    /// <param name="valueMatchers">The string values to wrap as matchers.</param>
    /// <returns>A <see cref="ValuesMatcherAll"/> requiring all values to match.</returns>
    protected static ValuesMatcherAll NewAllCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
        => new(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());

    /// <summary>Creates a <see cref="ValuesMatcherAny"/> whose matchers are constructed from the given string values using reflection on <typeparamref name="TEquatable"/>.</summary>
    /// <typeparam name="TEquatable">The <see cref="IEquatable{String}"/> type to instantiate for each value; must have a single-string constructor.</typeparam>
    /// <param name="valueMatchers">The string values to wrap as matchers.</param>
    /// <returns>A <see cref="ValuesMatcherAny"/> passing when any value matches.</returns>
    protected static ValuesMatcherAny NewAnyCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
        => new(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());

    /// <summary>Instantiates a <typeparamref name="TEquatable"/> from a single string value using its single-string constructor via reflection.</summary>
    /// <typeparam name="TEquatable">The <see cref="IEquatable{String}"/> type to instantiate; must have a single-string constructor.</typeparam>
    /// <param name="value">The string value to pass to the constructor.</param>
    /// <returns>A new <typeparamref name="TEquatable"/> instance wrapping <paramref name="value"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <typeparamref name="TEquatable"/> has no single-string constructor.</exception>
    protected static IEquatable<string> NewEquatable<TEquatable>(string value)
        where TEquatable : IEquatable<string>
    {
        ConstructorInfo ctor = typeof(TEquatable).GetConstructor([typeof(string)])
            ?? throw new InvalidOperationException($"No constructor of one string found for type {typeof(TEquatable).Name}");
        return (IEquatable<string>)ctor.Invoke([value]);
    }

    /// <summary>Adds a scoring criterion with a single-value evaluator to the current criterion list.</summary>
    /// <param name="priority">The relative importance of this criterion.</param>
    /// <param name="matcher">The matcher to apply to the extracted attribute value.</param>
    /// <param name="evaluator">Extracts a single attribute string from a discriminator instance.</param>
    /// <exception cref="InvalidOperationException">Thrown when a criterion with the same priority has already been added.</exception>
    protected void AddCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, string> evaluator)
        => AddCriterion(priority, matcher, e => [evaluator.Invoke(e)]);

    /// <summary>Adds a scoring criterion with a multi-value evaluator to the current criterion list.</summary>
    /// <param name="priority">The relative importance of this criterion.</param>
    /// <param name="matcher">The matcher to apply to the extracted attribute values.</param>
    /// <param name="evaluator">Extracts one or more attribute strings from a discriminator instance.</param>
    /// <exception cref="InvalidOperationException">Thrown when a criterion with the same priority has already been added.</exception>
    protected void AddCriterion(int priority, IValueMatcher matcher, Func<TDiscriminator, IEnumerable<string>> evaluator)
    {
        if (_criterionList.Any(c => c.Priority == priority))
            throw new InvalidOperationException("Criterion of equal priority already added");

        _criterionList.Add(new NodeScoreCriteriaSet<TDiscriminator>.NodeScoreCriterion(priority, matcher, evaluator));
    }
}
