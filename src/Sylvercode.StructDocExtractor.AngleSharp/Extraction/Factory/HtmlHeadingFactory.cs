using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlHeadingFactory(IOptions<HtmlHeadingFactory.Options>? options = null) : BaseAngleNodeFactory
{
    public class Options
    {
        public static Options Default { get; } = new Options();

        public bool UseTopHendingAsKey = false;

        public bool DeminishHeadingLevel = false;
    }

    private readonly IOptions<Options> _options = options
        ?? Microsoft.Extensions.Options.Options.Create(Options.Default);

    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithRegExTagName(@"h\d+")
            .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node is not IHtmlHeadingElement headingElement)
            throw new ArgumentException("Node is not an heading element", nameof(node));

        Options options = _options.Value;

        if (options.UseTopHendingAsKey)
            resultBuilder.WithMetadata(StdMetadata.PageTopHeadingKey, headingElement.TextContent);

        var level = int.Parse(headingElement.TagName[1..]);
        if (options.DeminishHeadingLevel)
            level--;

        if (level <= 0)
            return true;
            
        resultBuilder.WithNode(new HtmlHeading(level, node.Id ?? ""));
        resultBuilder.WithChildNodesAsSubTasks();

        return true;
    }
}
