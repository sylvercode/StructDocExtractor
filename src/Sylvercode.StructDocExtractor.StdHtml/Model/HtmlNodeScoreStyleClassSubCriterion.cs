using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlNodeScoreStyleClassSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeDiscriminator>.NodeScoreCriterion(1, matcher, n => n.StyleClass)
{

}
