using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlImgFactory : BaseAngleNodeFactory
{
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Img)
            .BuildSets();

    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node is not IHtmlImageElement img)
            throw new ArgumentException($"Node is not an {nameof(IHtmlImageElement)}", nameof(node));

        if (string.IsNullOrWhiteSpace(img.Source))
            return true;

        resultBuilder.WithNode(new HtmlImg(img.Source));
        return true;
    }
}
