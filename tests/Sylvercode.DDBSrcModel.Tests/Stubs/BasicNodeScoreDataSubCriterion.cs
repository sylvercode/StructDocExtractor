using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Tests.Stubs;

public class BasicNodeScoreDataSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<BasicNodeSelectable>.NodeScoreCriterion(0, matcher, n => n.Data)
{
    public static readonly BasicNodeScoreDataSubCriterion Instance = new(new ValuesMatcherAll(BasicNodeSelectable.DataValue1));
}
