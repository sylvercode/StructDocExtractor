using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;a&gt;</c> elements to <see cref="HtmlAnchor"/> nodes
/// with the resolved <c>href</c> URI.
/// </summary>
/// <remarks>
/// Elements with an empty or whitespace-only <c>href</c> are silently skipped (returns
/// <see langword="true"/> without producing a node), so bare anchors used as page targets
/// are not included in the output tree.
/// </remarks>
public class HtmlAnchorFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting <c>&lt;a&gt;</c> elements.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.A)
            .BuildSets();

    /// <inheritdoc/>
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
