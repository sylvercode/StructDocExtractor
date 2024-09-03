using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicNodeScoreDataSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<BasicNodeDiscriminator>.NodeScoreCriterion(0, matcher, n => n.Data)
{
    public static readonly BasicNodeScoreDataSubCriterion Instance = new(new ValuesMatcherAll(BasicNodeDiscriminator.DataValue1));
}
