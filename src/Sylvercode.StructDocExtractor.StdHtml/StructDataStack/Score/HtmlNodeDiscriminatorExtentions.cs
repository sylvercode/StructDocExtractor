using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

/// <summary>
/// Provides extension methods for <see cref="HtmlNodeDiscriminator"/> to build fluent score-criteria
/// objects used in HTML node factory selection.
/// </summary>
public static class HtmlNodeDiscriminatorExtentions
{
    /// <summary>
    /// Converts this <see cref="HtmlNodeDiscriminator"/> into a single
    /// <see cref="NodeScoreCriteriaSet{T}.NodeScoreCriteria"/> composed of non-null property criteria.
    /// </summary>
    /// <param name="discriminator">The discriminator whose properties define the matching criteria.</param>
    /// <returns>
    /// A <see cref="NodeScoreCriteriaSet{T}.NodeScoreCriteria"/> that scores nodes by ID, style class,
    /// and tag name, skipping any properties that are empty or null.
    /// </returns>
    public static NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriteria AsCriteria(this HtmlNodeDiscriminator discriminator)
    {
        List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriterion> criterionList = [];

        if (!string.IsNullOrEmpty(discriminator.Id))
            criterionList.Add(new(1, new ValuesMatcherAll(discriminator.Id), d => d.Id));

        if (discriminator.StyleClass.Length > 0)
            criterionList.Add(new(2, new ValuesMatcherAll(discriminator.StyleClass), d => d.StyleClass));

        if (!string.IsNullOrEmpty(discriminator.TagName))
            criterionList.Add(new(3, new ValuesMatcherAll(discriminator.TagName), d => d.TagName));

        return new(criterionList);
    }

    /// <summary>
    /// Wraps this <see cref="HtmlNodeDiscriminator"/> in a <see cref="NodeScoreCriteriaSet{T}"/> ready for use in a score calculator.
    /// </summary>
    /// <param name="discriminator">The discriminator to convert.</param>
    /// <returns>A <see cref="NodeScoreCriteriaSet{T}"/> containing a single criteria derived from this discriminator.</returns>
    public static NodeScoreCriteriaSet<HtmlNodeDiscriminator> AsCriteriaSet(this HtmlNodeDiscriminator discriminator)
        => new(discriminator.AsCriteria());

    /// <summary>
    /// Creates a <see cref="StackScoreCalculator{T}"/> that evaluates a single-element stack against this discriminator.
    /// </summary>
    /// <param name="discriminator">The discriminator to use as the sole criteria set.</param>
    /// <returns>A <see cref="StackScoreCalculator{T}"/> wrapping this discriminator's criteria set.</returns>
    public static StackScoreCalculator<HtmlNodeDiscriminator> AsScoreCalculator(this HtmlNodeDiscriminator discriminator)
        => new([discriminator.AsCriteriaSet()]);

    /// <summary>
    /// Creates a <see cref="StackScoreCalculator{T}"/> that evaluates each discriminator in the stack, one criteria set per element.
    /// </summary>
    /// <param name="discriminatorStack">The ordered array of discriminators forming the expected ancestor stack.</param>
    /// <returns>A <see cref="StackScoreCalculator{T}"/> built from one criteria set per discriminator in the stack.</returns>
    public static StackScoreCalculator<HtmlNodeDiscriminator> AsScoreCalculator(this HtmlNodeDiscriminator[] discriminatorStack)
        => new(discriminatorStack.Select(d => d.AsCriteriaSet()).ToArray());
}
