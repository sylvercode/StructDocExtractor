using System.Reflection;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

public class HtmlScoreCriteriaSetsBuilder
{
    public const int IdScorePriority = 1;
    public const int StyleClassScorePriority = 2;
    public const int TagNameScorePriority = 3;

    public static Func<HtmlNodeDiscriminator, string> IdGetter { get; } = d => d.Id;
    public static Func<HtmlNodeDiscriminator, string[]> StyleClassGetter { get; } = d => d.StyleClass;
    public static Func<HtmlNodeDiscriminator, string> TagNameGetter { get; } = d => d.TagName;

    private readonly List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriterion> _criterionList = [];

    private readonly List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriteria> _criteriaList = [];

    private readonly List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> _criteriaSetList = [];


    public HtmlScoreCriteriaSetsBuilder WithId(string id)
        => WithAnyId([id]);

    public HtmlScoreCriteriaSetsBuilder WithRegExId(string id)
    {
        AddIdCriterion(NewAnyCriterion<RegexValueMatcher>([id]));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithAnyId(string[] id)
    {
        AddIdCriterion(NewAnyCriterion<StaticValueMatcher>(id));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithStyleClass(string styleClass)
    {
        AddStyleClassCriterion(NewAnyCriterion<StaticValueMatcher>([styleClass]));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithRegExStyleClass(string styleClass)
    {
        AddStyleClassCriterion(NewAnyCriterion<RegexValueMatcher>([styleClass]));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithAllStyleClass(string[] styleClasses)
    {
        AddStyleClassCriterion(NewAllCriterion<StaticValueMatcher>(styleClasses));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithAnyStyleClass(string[] styleClasses)
    {
        AddStyleClassCriterion(NewAnyCriterion<StaticValueMatcher>(styleClasses));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithTagName(string tagName)
        => WithAnyTagName([tagName]);

    public HtmlScoreCriteriaSetsBuilder WithRegExTagName(string tagName)
    {
        AddTagCriterion(NewAnyCriterion<RegexValueMatcher>([tagName]));
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithAnyTagName(string[] tagName)
    {
        AddTagCriterion(NewAnyCriterion<StaticValueMatcher>(tagName));
        return this;
    }
    public HtmlScoreCriteriaSetsBuilder NextCriteria()
    {
        if (_criterionList.Count == 0)
            return this;
        _criteriaList.Add(new(_criterionList));
        _criterionList.Clear();
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder NextCriteriaSet()
    {
        NextCriteria();
        if (_criteriaList.Count == 0)
            return this;
        _criteriaSetList.Add(new(_criteriaList.ToArray()));
        _criteriaList.Clear();
        return this;
    }

    public HtmlScoreCriteriaSetsBuilder WithNextCriteriaSets(
        IEnumerable<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> criteriaSets)
    {
        NextCriteriaSet();
        _criteriaSetList.AddRange(criteriaSets);
        return this;
    }

    public List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Build()
    {
        NextCriteriaSet();
        return _criteriaSetList;
    }

    private static ValuesMatcherAll NewAllCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
    {
        return new ValuesMatcherAll(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());
    }

    private static ValuesMatcherAny NewAnyCriterion<TEquatable>(string[] valueMatchers)
        where TEquatable : IEquatable<string>
    {
        return new ValuesMatcherAny(valueMatchers.Select(NewEquatable<TEquatable>).ToArray());
    }

    private static IEquatable<string> NewEquatable<TEquatable>(string value)
        where TEquatable : IEquatable<string>
    {
        ConstructorInfo ctor = typeof(TEquatable).GetConstructor([typeof(string)])
            ?? throw new InvalidOperationException();
        return (IEquatable<string>)ctor.Invoke([value]);
    }

    private void AddIdCriterion(IValueMatcher valueMatcher)
    {
        CheckRepeatedCriterion(IdScorePriority);
        _criterionList.Add(new(IdScorePriority, valueMatcher, IdGetter));
    }

    private void AddStyleClassCriterion(IValueMatcher valueMatcher)
    {
        CheckRepeatedCriterion(StyleClassScorePriority);
        _criterionList.Add(new(StyleClassScorePriority, valueMatcher, StyleClassGetter));
    }

    private void AddTagCriterion(IValueMatcher valueMatcher)
    {
        CheckRepeatedCriterion(TagNameScorePriority);
        _criterionList.Add(new(TagNameScorePriority, valueMatcher, TagNameGetter));
    }

    private void CheckRepeatedCriterion(int priority)
    {
        if (_criterionList.Any(c => c.Priority == priority))
            throw new InvalidOperationException("Criterion of equal priority already added");
    }
}
