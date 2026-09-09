using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;figcaption&gt;</c> elements to <see cref="HtmlFigCaption"/> nodes.
/// </summary>
public class HtmlFigCaptionFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;figcaption&gt;</c> elements.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Figcaption)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlFigCaption>();
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
