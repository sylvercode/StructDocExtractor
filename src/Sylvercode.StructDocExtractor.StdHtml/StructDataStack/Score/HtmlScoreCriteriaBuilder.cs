using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

public class HtmlScoreCriteriaSetsBuilder : BaseScoreCriteriaSetsBuilder<HtmlNodeDiscriminator, HtmlScoreCriteriaSetsBuilder>
{
    public const int IdScorePriority = 2;
    public const int StyleClassScorePriority = 1;
    public const int TagNameScorePriority = 0;

    public static Func<HtmlNodeDiscriminator, string> IdGetter { get; } = d => d.Id;
    public static Func<HtmlNodeDiscriminator, string[]> StyleClassGetter { get; } = d => d.StyleClass;
    public static Func<HtmlNodeDiscriminator, string> TagNameGetter { get; } = d => d.TagName;


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

    private void AddIdCriterion(IValueMatcher valueMatcher)
        => AddCriterion(IdScorePriority, valueMatcher, IdGetter);

    private void AddStyleClassCriterion(IValueMatcher valueMatcher)
        => AddCriterion(StyleClassScorePriority, valueMatcher, StyleClassGetter);

    private void AddTagCriterion(IValueMatcher valueMatcher)
        => AddCriterion(TagNameScorePriority, valueMatcher, TagNameGetter);
}
