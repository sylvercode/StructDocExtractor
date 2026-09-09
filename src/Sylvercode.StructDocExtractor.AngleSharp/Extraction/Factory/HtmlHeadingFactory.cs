using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp heading elements (<c>h1</c>–<c>h6</c>) into
/// <see cref="HtmlHeading"/> nodes with their parsed heading level.
/// </summary>
/// <remarks>
/// Behaviour is controlled by an optional <see cref="Options"/> instance:
/// <see cref="Options.UseTopHendingAsKey"/> stores the <c>h1</c> text as a
/// <see cref="StdMetadata.PageTopHeadingKey"/> metadata entry, and
/// <see cref="Options.DeminishHeadingLevel"/> decrements each level by one before
/// creating the node (useful for sub-page extraction). Heading levels that reduce to zero
/// or below are silently dropped — no node is produced.
/// </remarks>
public class HtmlHeadingFactory(HtmlHeadingFactory.Options? options = null) : BaseAngleNodeFactory
{
    /// <summary>Configuration options that alter <see cref="HtmlHeadingFactory"/> behaviour.</summary>
    public class Options
    {
        /// <summary>Gets the default options instance with all flags disabled.</summary>
        public static Options Default { get; } = new Options();

        /// <summary>
        /// Gets a value indicating whether the text of top-level (<c>h1</c>) headings
        /// should be stored as the <see cref="StdMetadata.PageTopHeadingKey"/> metadata entry.
        /// </summary>
        public bool UseTopHendingAsKey { get; init; } = false;

        /// <summary>
        /// Gets a value indicating whether each heading level should be decremented by one,
        /// effectively promoting all headings one level toward the root.
        /// </summary>
        public bool DeminishHeadingLevel { get; init; } = false;
    }

    private readonly Options _options = options
        ?? Options.Default;

    /// <summary>Gets the default selector criteria targeting any <c>h&lt;n&gt;</c> element.</summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithRegExTagName(@"h\d+")
            .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node is not IHtmlHeadingElement headingElement)
            throw new ArgumentException("Node is not an heading element", nameof(node));

        var level = int.Parse(headingElement.TagName[1..]);
        if (level == 1
            && _options.UseTopHendingAsKey)
            resultBuilder.WithMetadata(StdMetadata.PageTopHeadingKey, headingElement.TextContent);


        if (_options.DeminishHeadingLevel)
            level--;

        if (level <= 0)
            return true;

        resultBuilder.WithNode(new HtmlHeading(level, node.Id ?? ""));
        resultBuilder.WithChildNodesAsSubTasks();

        return true;
    }
}
