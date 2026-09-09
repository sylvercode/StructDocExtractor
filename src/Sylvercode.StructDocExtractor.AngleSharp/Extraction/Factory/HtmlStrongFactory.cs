using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;strong&gt;</c> elements to <see cref="HtmlStrong"/> nodes.
/// </summary>
public class HtmlStrongFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the default selector criteria targeting <c>&lt;strong&gt;</c> elements.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Strong)
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlStrong>();
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
