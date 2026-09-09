using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;img&gt;</c> elements to <see cref="HtmlImg"/> nodes
/// with the resolved <c>src</c> URI.
/// </summary>
/// <remarks>
/// Images whose <c>Source</c> resolves to an empty or whitespace-only string are skipped,
/// producing no node in the output tree.
/// </remarks>
public class HtmlImgFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting <c>&lt;img&gt;</c> elements.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Img)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
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
