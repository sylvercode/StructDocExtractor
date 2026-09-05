using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;li&gt;</c> elements to <see cref="HtmlListItem"/> nodes.
/// </summary>
/// <remarks>
/// The default selector requires the parent to match <see cref="HtmlListFactory.Selector"/>,
/// ensuring list items are only extracted under a recognised list container.
/// </remarks>
public class HtmlListItemFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting <c>&lt;li&gt;</c> elements inside a list.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Li)
            .AndParentCriteriaSets(HtmlListFactory.Selector)
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlListItem>();
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
