using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class PlainTextNodeFactory : BaseAngleNodeFactory
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(HtmlNodeDiscriminator.PlainTextTagName)
            .BuildSets();

    protected override bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
    {
        resultBuilder.WithNode(new PlainTextNode(node.TextContent));

        return true;
    }
}
