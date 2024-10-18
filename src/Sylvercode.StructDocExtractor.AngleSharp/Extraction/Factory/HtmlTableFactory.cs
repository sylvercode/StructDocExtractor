using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlTableFactory : BaseAngleNodeFactory
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Table)
            .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlTable>();
        resultBuilder.WithSubTaskByAll(TagNames.Thead);
        resultBuilder.WithSubTaskByAll(TagNames.Tbody);
        resultBuilder.WithSubTaskByAll(TagNames.Tfoot);
        resultBuilder.WithSubTaskByAll(TagNames.Tr);
        return true;
    }
}
