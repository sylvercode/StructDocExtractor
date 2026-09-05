using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;td&gt;</c> elements to <see cref="HtmlTableRowData"/> nodes.
/// </summary>
/// <remarks>
/// The default selector requires a parent matching <see cref="HtmlTableRowFactory.Selector"/>,
/// ensuring data cells are only extracted inside recognised table rows.
/// </remarks>
public class HtmlTableRowDataFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;td&gt;</c> elements inside a table row.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Td)
            .AndParentCriteriaSets(HtmlTableRowFactory.Selector)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTableRowData>();
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
