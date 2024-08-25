using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Html;

public class HtmlNodeScoreStyleClassSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeSelectable>.NodeScoreCriterion(1, matcher, n => n.StyleClass)
{

}
