using Sylvercode.DDBSrcModel.StructDocStack.Score;

namespace Sylvercode.DDBSrcModel.Html;

public class HtmlNodeScoreTagSubCriterion(IValueMatcher matcher)
        : NodeScoreCriteriaSet<HtmlNodeSelectable>.NodeScoreCriterion(2, matcher, n => n.TagName)
{

}
