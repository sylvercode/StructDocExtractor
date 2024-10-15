using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction;

public class AngleProcessTaskResultBuilder(IElement sourceNode) : ProcessTaskResultBuilder<IElement, HtmlNodeDiscriminator>
{
    public AngleProcessTaskResultBuilder WithSubTaskByAll(string selector)
    {
        WithSubTasks(sourceNode.QuerySelectorAll(selector));
        return this;
    }
    public AngleProcessTaskResultBuilder WithSubTaskBySingle(string selector)
    {
        IElement? subElement = sourceNode.QuerySelector(selector);
        if (subElement is not null)
            WithSubTask(subElement);
        return this;
    }

    public AngleProcessTaskResultBuilder WithExtraTaskByAll(string selector)
    {
        WithExtraTasks(sourceNode.QuerySelectorAll(selector));
        return this;
    }
    
    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(string selector)
    {
        IElement? extraElement = sourceNode.QuerySelector(selector);
        if (extraElement is not null)
            WithExtraTask(extraElement);
        return this;
    }
}
