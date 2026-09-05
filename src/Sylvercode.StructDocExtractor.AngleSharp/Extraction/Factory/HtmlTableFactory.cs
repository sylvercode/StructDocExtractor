using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;table&gt;</c> elements to <see cref="HtmlTable"/> nodes.
/// </summary>
/// <remarks>
/// Dispatches direct child <c>&lt;thead&gt;</c>, <c>&lt;tbody&gt;</c>, <c>&lt;tfoot&gt;</c>, and
/// top-level <c>&lt;tr&gt;</c> elements as separate sub-tasks to support both sectioned and
/// unsectioned table structures.
/// </remarks>
public class HtmlTableFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;table&gt;</c> elements.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Table)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTable>();
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Thead}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tbody}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tfoot}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tr}");
        return true;
    }
}
