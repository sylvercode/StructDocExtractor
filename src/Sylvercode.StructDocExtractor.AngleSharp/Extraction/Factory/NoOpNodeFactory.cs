using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that intentionally skips nodes mapped to an empty discriminator.
/// </summary>
/// <remarks>
/// This absorbs non-meaningful nodes (for example whitespace-only text and non-element nodes)
/// so the sequencer does not report a missing node factory for them.
/// </remarks>
public class NoOpNodeFactory : BaseAngleNodeFactory
{
    /// <summary>Gets the selector targeting discriminators whose tag name is empty.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(string.Empty)
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
        => true;
}
