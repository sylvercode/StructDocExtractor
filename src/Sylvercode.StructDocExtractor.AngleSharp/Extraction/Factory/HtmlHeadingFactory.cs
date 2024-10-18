using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlHeadingFactory : BaseAngleNodeFactory
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithRegExTagName(@"h\d+")
            .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node is not IHtmlHeadingElement headingElement)
            throw new ArgumentException("Node is not an anchor element", nameof(node));

        var level = int.Parse(headingElement.TagName[1..]);
        resultBuilder.WithNode(new HtmlHeading(level));

        resultBuilder.WithChildNodesAsSubTasks();

        return true;
    }
}
