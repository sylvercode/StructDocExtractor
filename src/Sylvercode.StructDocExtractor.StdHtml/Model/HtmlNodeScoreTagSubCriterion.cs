using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlNodeScoreTagSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeSelectable>.NodeScoreCriterion(2, matcher, n => n.TagName)
{

}
