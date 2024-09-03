using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicNodeScoreIdSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<BasicNodeSelectable>.NodeScoreCriterion(1, matcher, n => n.Id)
{
    public static readonly BasicNodeScoreIdSubCriterion ByDefaultId = new(new ValuesMatcherAll(BasicNodeSelectable.DefautltId));
    public static readonly BasicNodeScoreIdSubCriterion ByDefaultParentId = new(new ValuesMatcherAll(BasicNodeSelectable.DefautltParentId));
}
