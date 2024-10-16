using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction;

public class AngleProcessTaskResultBuilder(IElement sourceNode) : ProcessTaskResultBuilder<IElement, HtmlNodeDiscriminator>
{
    public AngleProcessTaskResultBuilder WithSubTaskByAll(string selector)
        => WithSubTaskByAll(sourceNode, selector);

    public AngleProcessTaskResultBuilder WithSubTaskByAll(IElement element, string selector)
    {
        WithSubTasks(element.QuerySelectorAll(selector));
        return this;
    }

    public AngleProcessTaskResultBuilder WithSubTaskBySingle(string selector)
        => WithSubTaskBySingle(sourceNode, selector);

    public AngleProcessTaskResultBuilder WithSubTaskBySingle(IElement element, string selector)
    {
        IElement? subElement = element.QuerySelector(selector);
        if (subElement is not null)
            WithSubTask(subElement);
        return this;
    }

    public AngleProcessTaskResultBuilder WithExtraTaskByAll(string selector)
        => WithExtraTaskByAll(sourceNode, selector);

    public AngleProcessTaskResultBuilder WithExtraTaskByAll(IElement element, string selector)
    {
        WithExtraTasks(element.QuerySelectorAll(selector));
        return this;
    }

    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(string selector)
        => WithExtraTaskBySingle(sourceNode, selector);

    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(IElement element, string selector)
    {
        IElement? extraElement = element.QuerySelector(selector);
        if (extraElement is not null)
            WithExtraTask(extraElement);
        return this;
    }
}
