using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlTableFactory : BaseAngleNodeFactory
{
    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Table)
            .BuildSets();

    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } = Selector;

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTable>();
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Thead}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tbody}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tfoot}");
        resultBuilder.WithSubTaskByAll($":scope>{TagNames.Tr}");
        return true;
    }
}
