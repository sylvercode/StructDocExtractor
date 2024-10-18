using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlAnchorFactory : BaseAngleNodeFactory
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.A)
            .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node is not IHtmlAnchorElement anchorElement)
            throw new ArgumentException($"Node is not an {nameof(IHtmlAnchorElement)}.", nameof(node));

        if (string.IsNullOrWhiteSpace(anchorElement.Href))
            return true;

        resultBuilder.WithNode(new HtmlAnchor(anchorElement.Href));
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
