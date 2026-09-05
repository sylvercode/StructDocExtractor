using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;tr&gt;</c> elements to <see cref="HtmlTableRow"/> nodes.
/// </summary>
/// <remarks>
/// The default selector accepts <c>&lt;tr&gt;</c> elements whose parent is a <c>&lt;table&gt;</c>,
/// <c>&lt;thead&gt;</c>, <c>&lt;tbody&gt;</c>, or <c>&lt;tfoot&gt;</c>, covering both sectioned
/// and unsectioned table layouts.
/// </remarks>
public class HtmlTableRowFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;tr&gt;</c> elements inside any table section.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Tr)
            .AndParent().WithTagName(TagNames.Table)
                .Or().WithTagName(TagNames.Thead)
                .Or().WithTagName(TagNames.Tbody)
                .Or().WithTagName(TagNames.Tfoot)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTableRow>();
        resultBuilder.WithSubTaskByAll(TagNames.Th);
        resultBuilder.WithSubTaskByAll(TagNames.Td);
        return true;
    }
}
