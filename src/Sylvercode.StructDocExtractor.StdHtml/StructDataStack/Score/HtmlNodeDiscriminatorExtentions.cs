using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

public static class HtmlNodeDiscriminatorExtentions
{
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

    public static NodeScoreCriteriaSet<HtmlNodeDiscriminator> AsCriteriaSet(this HtmlNodeDiscriminator discriminator)
        => new(discriminator.AsCriteria());

    public static StackScoreCalculator<HtmlNodeDiscriminator> AsScoreCalculator(this HtmlNodeDiscriminator discriminator)
        => new([discriminator.AsCriteriaSet()]);
}
