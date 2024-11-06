using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlHeadingFactory(HtmlHeadingFactory.Options? options = null) : BaseAngleNodeFactory
{
    public class Options
    {
        public static Options Default { get; } = new Options();

        public bool UseTopHendingAsKey { get; init; } = false;

        public bool DeminishHeadingLevel { get; init; } = false;
    }

    private readonly Options _options = options
        ?? Options.Default;

    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithRegExTagName(@"h\d+")
            .BuildSets();

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
