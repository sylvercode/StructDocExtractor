using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp text nodes to <see cref="PlainTextNode"/> instances.
/// </summary>
/// <remarks>
/// Matches DOM nodes whose tag name equals <see cref="HtmlNodeDiscriminator.PlainTextTagName"/>
/// and reads the node's <c>TextContent</c> directly, bypassing the element cast used by other factories.
/// </remarks>
public class PlainTextNodeFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting plain-text virtual nodes.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(HtmlNodeDiscriminator.PlainTextTagName)
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
    {
        resultBuilder.WithNode(new PlainTextNode(node.TextContent));

        return true;
    }
}
