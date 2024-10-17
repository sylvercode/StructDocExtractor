using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public abstract class BaseAngleNodeFactory : IHtmlNodeFactory<INode>
{
    public virtual List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; }

    public IProcessTaskResult<INode, HtmlNodeDiscriminator> NewNode(HtmlNodeDiscriminator discriminator, INode data)
    {
        AngleProcessTaskResultBuilder builder = new(data);
        builder.WithDataDiscriminator(discriminator);

        return BuildFromNode(builder, data) ? builder.Build() : ProcessTaskResult.NewError<INode, HtmlNodeDiscriminator>();
    }

    protected virtual bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
    {
        if (node is not IElement element)
            return false;

        return BuildFromElement(resultBuilder, element);
    }

    protected virtual bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement element)
    {
        throw new NotImplementedException();
    }
}
