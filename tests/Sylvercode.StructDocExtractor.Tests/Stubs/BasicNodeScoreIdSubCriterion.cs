using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicNodeScoreIdSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<BasicNodeDiscriminator>.NodeScoreCriterion(1, matcher, n => n.Id)
{
    public static readonly BasicNodeScoreIdSubCriterion ByDefaultId = new(new ValuesMatcherAll(BasicNodeDiscriminator.DefaultId));
    public static readonly BasicNodeScoreIdSubCriterion ByDefaultParentId = new(new ValuesMatcherAll(BasicNodeDiscriminator.DefaultParentId));
}
