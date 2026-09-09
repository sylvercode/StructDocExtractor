using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction;

/// <summary>
/// Fluent builder for <see cref="IProcessTaskResult{TData,TDiscriminator}"/> objects scoped to
/// AngleSharp <see cref="INode"/> and <see cref="HtmlNodeDiscriminator"/>.
/// </summary>
/// <remarks>
/// Extends <see cref="ProcessTaskResultBuilder{TData,TDiscriminator}"/> with AngleSharp-specific
/// helpers for scheduling child nodes and CSS-selector-matched elements as sub-tasks or extra-tasks.
/// The <c>ElementNode</c> property throws <see cref="InvalidOperationException"/> if the wrapped
/// source node is not an <see cref="IElement"/>, so selector-based methods must only be called when
/// the source node is an element.
/// </remarks>
public class AngleProcessTaskResultBuilder(INode sourceNode) : ProcessTaskResultBuilder<INode, HtmlNodeDiscriminator>
{
    private IElement ElementNode
    {
        get
        {
            return sourceNode as IElement ?? throw new InvalidOperationException("Node is not an element");
        }
    }

    /// <summary>Schedules all direct child nodes of the source node as sub-tasks.</summary>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithChildNodesAsSubTasks()
        => WithChildNodesAsSubTasks(sourceNode);

    /// <summary>Schedules all direct child nodes of <paramref name="node"/> as sub-tasks.</summary>
    /// <param name="node">The node whose children to schedule.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithChildNodesAsSubTasks(INode node)
    {
        WithSubTasks(node.ChildNodes);
        return this;
    }

    /// <summary>Schedules all direct child nodes of the source node as extra tasks.</summary>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithChildNodesAsExtraTasks()
        => WithChildNodesAsExtraTasks(sourceNode);

    /// <summary>Schedules all direct child nodes of <paramref name="node"/> as extra tasks.</summary>
    /// <param name="node">The node whose children to schedule.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithChildNodesAsExtraTasks(INode node)
    {
        WithSubTasks(node.ChildNodes);
        return this;
    }

    /// <summary>
    /// Schedules all elements matching <paramref name="selector"/> within the source element as sub-tasks.
    /// </summary>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithSubTaskByAll(string selector)
        => WithSubTaskByAll(ElementNode, selector);

    /// <summary>
    /// Schedules all elements matching <paramref name="selector"/> within <paramref name="element"/> as sub-tasks.
    /// </summary>
    /// <param name="element">The element to query.</param>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithSubTaskByAll(IElement element, string selector)
    {
        WithSubTasks(element.QuerySelectorAll(selector));
        return this;
    }

    /// <summary>
    /// Schedules the first element matching <paramref name="selector"/> within the source element as a sub-task,
    /// if found.
    /// </summary>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithSubTaskBySingle(string selector)
        => WithSubTaskBySingle(ElementNode, selector);

    /// <summary>
    /// Schedules the first element matching <paramref name="selector"/> within <paramref name="element"/>
    /// as a sub-task, if found.
    /// </summary>
    /// <param name="element">The element to query.</param>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithSubTaskBySingle(IElement element, string selector)
    {
        INode? subElement = element.QuerySelector(selector);
        if (subElement is not null)
            WithSubTask(subElement);
        return this;
    }

    /// <summary>
    /// Schedules all elements matching <paramref name="selector"/> within the source element as extra tasks.
    /// </summary>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithExtraTaskByAll(string selector)
        => WithExtraTaskByAll(ElementNode, selector);

    /// <summary>
    /// Schedules all elements matching <paramref name="selector"/> within <paramref name="element"/> as extra tasks.
    /// </summary>
    /// <param name="element">The element to query.</param>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithExtraTaskByAll(IElement element, string selector)
    {
        WithExtraTasks(element.QuerySelectorAll(selector));
        return this;
    }

    /// <summary>
    /// Schedules the first element matching <paramref name="selector"/> within the source element as an extra task,
    /// if found.
    /// </summary>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(string selector)
        => WithExtraTaskBySingle(ElementNode, selector);

    /// <summary>
    /// Schedules the first element matching <paramref name="selector"/> within <paramref name="element"/>
    /// as an extra task, if found.
    /// </summary>
    /// <param name="element">The element to query.</param>
    /// <param name="selector">A CSS selector string.</param>
    /// <returns>This builder instance for chaining.</returns>
    public AngleProcessTaskResultBuilder WithExtraTaskBySingle(IElement element, string selector)
    {
        INode? extraElement = element.QuerySelector(selector);
        if (extraElement is not null)
            WithExtraTask(extraElement);
        return this;
    }
}
