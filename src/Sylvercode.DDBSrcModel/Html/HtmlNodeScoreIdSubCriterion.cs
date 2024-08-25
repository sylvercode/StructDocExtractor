using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Html;

public class HtmlNodeScoreIdSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeSelectable>.NodeScoreCriterion(0, matcher, n => n.Id)
{

}
