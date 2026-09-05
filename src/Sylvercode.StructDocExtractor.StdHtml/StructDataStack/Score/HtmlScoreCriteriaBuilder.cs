using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

/// <summary>
/// Fluent builder for assembling HTML-specific <see cref="NodeScoreCriteriaSet{T}"/> collections
/// that evaluate <see cref="HtmlNodeDiscriminator"/> instances by tag name, CSS class, and ID.
/// </summary>
/// <remarks>
/// Extends <see cref="BaseScoreCriteriaSetsBuilder{TStackDataDiscriminator,TBuilder}"/> with HTML attribute
/// getters and priority constants, providing strongly-typed <c>With*</c> methods for building scoring
/// rules against static values or regular expressions. The resulting criteria sets drive
/// <see cref="StackScoreCalculator{T}"/> instances used by <see cref="HtmlNodeFactoryProvider{TExtractionData}"/>
/// to resolve the correct factory for a given DOM traversal context.
/// </remarks>
public class HtmlScoreCriteriaSetsBuilder : BaseScoreCriteriaSetsBuilder<HtmlNodeDiscriminator, HtmlScoreCriteriaSetsBuilder>
{
    /// <summary>Gets the scoring priority applied to element ID criteria.</summary>
    public const int IdScorePriority = 2;
    /// <summary>Gets the scoring priority applied to CSS class criteria.</summary>
    public const int StyleClassScorePriority = 1;
    /// <summary>Gets the scoring priority applied to tag name criteria.</summary>
    public const int TagNameScorePriority = 0;

    /// <summary>Gets the accessor that extracts the <c>Id</c> from a <see cref="HtmlNodeDiscriminator"/>.</summary>
    public static Func<HtmlNodeDiscriminator, string> IdGetter { get; } = d => d.Id;
    /// <summary>Gets the accessor that extracts the <c>StyleClass</c> array from a <see cref="HtmlNodeDiscriminator"/>.</summary>
    public static Func<HtmlNodeDiscriminator, string[]> StyleClassGetter { get; } = d => d.StyleClass;
    /// <summary>Gets the accessor that extracts the <c>TagName</c> from a <see cref="HtmlNodeDiscriminator"/>.</summary>
    public static Func<HtmlNodeDiscriminator, string> TagNameGetter { get; } = d => d.TagName;


    /// <summary>Adds an exact-match ID criterion for the given <paramref name="id"/>.</summary>
    /// <param name="id">The element ID to match.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithId(string id)
        => WithAnyId([id]);

    /// <summary>Adds a regex ID criterion matching elements whose ID satisfies <paramref name="id"/>.</summary>
    /// <param name="id">The regular expression pattern to match against the element ID.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithRegExId(string id)
    {
        AddIdCriterion(NewAnyCriterion<RegexValueMatcher>([id]));
        return this;
    }

    /// <summary>Adds an exact-match criterion that passes when the element ID matches any value in <paramref name="id"/>.</summary>
    /// <param name="id">The candidate ID values; the criterion passes when the element ID equals any of them.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithAnyId(string[] id)
    {
        AddIdCriterion(NewAnyCriterion<StaticValueMatcher>(id));
        return this;
    }

    /// <summary>Adds an exact-match style-class criterion for the given <paramref name="styleClass"/>.</summary>
    /// <param name="styleClass">The CSS class name the element must have.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithStyleClass(string styleClass)
    {
        AddStyleClassCriterion(NewAnyCriterion<StaticValueMatcher>([styleClass]));
        return this;
    }

    /// <summary>Adds a regex style-class criterion matching elements that have a class satisfying <paramref name="styleClass"/>.</summary>
    /// <param name="styleClass">The regular expression pattern to match against the element's CSS classes.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithRegExStyleClass(string styleClass)
    {
        AddStyleClassCriterion(NewAnyCriterion<RegexValueMatcher>([styleClass]));
        return this;
    }

    /// <summary>Adds a style-class criterion that requires the element to have <em>all</em> classes in <paramref name="styleClasses"/>.</summary>
    /// <param name="styleClasses">The CSS class names that must all be present on the element.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithAllStyleClass(string[] styleClasses)
    {
        AddStyleClassCriterion(NewAllCriterion<StaticValueMatcher>(styleClasses));
        return this;
    }

    /// <summary>Adds a style-class criterion that passes when the element has <em>any</em> class in <paramref name="styleClasses"/>.</summary>
    /// <param name="styleClasses">The CSS class names; the criterion passes when at least one is present.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithAnyStyleClass(string[] styleClasses)
    {
        AddStyleClassCriterion(NewAnyCriterion<StaticValueMatcher>(styleClasses));
        return this;
    }

    /// <summary>Adds an exact-match tag name criterion for the given <paramref name="tagName"/>.</summary>
    /// <param name="tagName">The HTML tag name (e.g., <c>"div"</c>, <c>"p"</c>) the element must have.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithTagName(string tagName)
        => WithAnyTagName([tagName]);

    /// <summary>Adds a regex tag name criterion matching elements whose tag name satisfies <paramref name="tagName"/>.</summary>
    /// <param name="tagName">The regular expression pattern to match against the element's tag name.</param>
    /// <returns>This builder instance for chaining.</returns>
    public HtmlScoreCriteriaSetsBuilder WithRegExTagName(string tagName)
    {
        AddTagCriterion(NewAnyCriterion<RegexValueMatcher>([tagName]));
        return this;
    }

    /// <summary>Adds an exact-match criterion that passes when the element tag matches any value in <paramref name="tagName"/>.</summary>
    /// <param name="tagName">The candidate tag names; the criterion passes when the element tag equals any of them.</param>
    /// <returns>This builder instance for chaining.</returns>
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
