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
            .Build();

    protected override bool BuildNode(AngleProcessTaskResultBuilder resultBuilder, IElement element)
    {
        if (element is not IHtmlHeadingElement headingElement)
            throw new ArgumentException("Element is not an anchor element", nameof(element));

        var level = int.Parse(headingElement.TagName[1..]);
        resultBuilder.WithNode(new HtmlHeading(level));

        // resultBuilder.WithSubTask(element.ChildNodes);

        return true;
    }
}
