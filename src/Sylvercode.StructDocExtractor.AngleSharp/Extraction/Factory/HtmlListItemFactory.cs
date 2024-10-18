using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlListItemFactory : BaseAngleNodeFactory
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
            .WithTagName(TagNames.Li)
            .WithNextCriteriaSets(HtmlListFactory.Selector)
            .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        resultBuilder.WithNode<HtmlListItem>();
        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }
}
