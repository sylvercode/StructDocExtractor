using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class BaseParagraphFactory<TParaNode>(string tagName) : BaseAngleNodeFactory
    where TParaNode : IStructDocNode, new()
{
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        new HtmlScoreCriteriaSetsBuilder()
                .WithTagName(tagName)
                .BuildSets();

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node.Id is not null)
            resultBuilder.WithNode(NewNodeWithId(node.Id));
        else
            resultBuilder.WithNode<TParaNode>();

        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }

    protected virtual IStructDocNode NewNodeWithId(string id)
    {
        return new TParaNode
        {
            Id = id
        };
    }
}
