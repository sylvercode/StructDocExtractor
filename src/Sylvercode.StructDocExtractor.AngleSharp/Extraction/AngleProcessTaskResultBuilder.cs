using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction;

public class AngleProcessTaskResultBuilder(INode? sourceNode) : ProcessTaskResultBuilder<INode, HtmlNodeDiscriminator>
{
    private IElement ElementNode
    {
        get
        {
            return sourceNode as IElement ?? throw new InvalidOperationException("Node is not an element");
        }
    }
    
    public AngleProcessTaskResultBuilder WithSubTaskByAll(string selector)
        => WithSubTaskByAll(ElementNode, selector);

    public AngleProcessTaskResultBuilder WithSubTaskByAll(IElement element, string selector)
    {
        WithSubTasks(element.QuerySelectorAll(selector));
        return this;
    }

    public AngleProcessTaskResultBuilder WithSubTaskBySingle(string selector)
        => WithSubTaskBySingle(ElementNode, selector);

    public AngleProcessTaskResultBuilder WithSubTaskBySingle(IElement element, string selector)
    {
        INode? subElement = element.QuerySelector(selector);
        if (subElement is not null)
            WithSubTask(subElement);
        return this;
    }

    public AngleProcessTaskResultBuilder WithExtraTaskByAll(string selector)
        => WithExtraTaskByAll(ElementNode, selector);

    public AngleProcessTaskResultBuilder WithExtraTaskByAll(IElement element, string selector)
    {
        WithExtraTasks(element.QuerySelectorAll(selector));
        return this;
    }

    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(string selector)
        => WithExtraTaskBySingle(ElementNode, selector);

    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(IElement element, string selector)
    {
        INode? extraElement = element.QuerySelector(selector);
        if (extraElement is not null)
            WithExtraTask(extraElement);
        return this;
    }
}
