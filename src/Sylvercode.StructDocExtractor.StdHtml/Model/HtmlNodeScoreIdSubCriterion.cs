using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlNodeScoreIdSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriterion(0, matcher, n => n.Id)
{

}
