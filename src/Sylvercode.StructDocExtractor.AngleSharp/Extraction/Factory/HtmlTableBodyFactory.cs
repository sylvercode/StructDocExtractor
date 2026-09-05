using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;tbody&gt;</c> elements to <see cref="HtmlTableBody"/> nodes.
/// </summary>
/// <remarks>
/// The default selector requires a parent matching <see cref="HtmlTableFactory.Selector"/>,
/// so only <c>&lt;tbody&gt;</c> elements nested directly inside a table are processed.
/// </remarks>
public class HtmlTableBodyFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;tbody&gt;</c> inside a table.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Tbody)
            .AndParentCriteriaSets(HtmlTableFactory.Selector)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTableBody>();
        resultBuilder.WithSubTaskByAll(TagNames.Tr);
        return true;
    }
}
