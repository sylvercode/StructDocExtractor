using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;br&gt;</c> elements to <see cref="HtmlBr"/> nodes.
/// </summary>
/// <remarks>
/// Overrides <c>BuildFromNode</c> rather than <c>BuildFromElement</c> because a <c>&lt;br&gt;</c>
/// carries no attributes; the node is created unconditionally without an element cast.
/// </remarks>
public class HtmlBrFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting <c>&lt;br&gt;</c> elements.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Br)
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
    {
        resultBuilder.WithNode<HtmlBr>();

        return true;
    }
}
