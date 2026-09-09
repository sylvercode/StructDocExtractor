using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;ul&gt;</c> or <c>&lt;ol&gt;</c> elements to
/// <see cref="HtmlList"/> nodes.
/// </summary>
/// <remarks>
/// Uses a scoped CSS selector (<c>:scope&gt;li</c>) to dispatch only direct child
/// <c>&lt;li&gt;</c> elements as sub-tasks, preventing nested list items from being processed
/// at the wrong depth.
/// </remarks>
public class HtmlListFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the shared selector criteria targeting <c>&lt;ul&gt;</c> or <c>&lt;ol&gt;</c> elements.</summary>
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Ul).Or().WithTagName(TagNames.Ol)
            .BuildSets();

    /// <inheritdoc/>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;



    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlList>();
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Li}");
        return true;
    }
}
