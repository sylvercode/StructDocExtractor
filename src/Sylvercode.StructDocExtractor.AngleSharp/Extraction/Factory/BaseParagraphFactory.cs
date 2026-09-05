using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Generic abstract base factory that creates paragraph-level <see cref="IStructDocNode"/> instances
/// of type <typeparamref name="TParaNode"/> from HTML elements.
/// </summary>
/// <typeparam name="TParaNode">
/// The concrete <see cref="IStructDocNode"/> type to create; must have a public parameterless constructor.
/// </typeparam>
/// <remarks>
/// Configures the <see cref="BaseAngleNodeFactory.DefaultSelector"/> from the supplied tag name and
/// preserves the HTML element's <c>id</c> attribute by calling <see cref="NewNodeWithId"/> when
/// present. Child elements are always scheduled as sub-tasks for recursive extraction.
/// Subclasses may override <see cref="NewNodeWithId"/> to customise node instantiation when an id
/// is present.
/// </remarks>
public class BaseParagraphFactory<TParaNode>(string tagName = "") : BaseAngleNodeFactory
    where TParaNode : IStructDocNode, new()
{
    /// <summary>
    /// Gets the default selector criteria targeting elements with the configured tag name,
    /// or <see langword="null"/> if no tag name was specified.
    /// </summary>
    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; } =
        string.IsNullOrWhiteSpace(tagName)
            ? null
            : new HtmlScoreCriteriaSetsBuilder()
                .WithTagName(tagName)
                .BuildSets();

    /// <inheritdoc/>
    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement node)
    {
        if (node.Id is not null)
            resultBuilder.WithNode(NewNodeWithId(node.Id));
        else
            resultBuilder.WithNode<TParaNode>();

        resultBuilder.WithChildNodesAsSubTasks();
        return true;
    }

    /// <summary>
    /// Creates a new <typeparamref name="TParaNode"/> pre-populated with the given element id.
    /// </summary>
    /// <param name="id">The HTML element id to assign to the node.</param>
    /// <returns>A new <typeparamref name="TParaNode"/> with <see cref="IStructDocNode.Id"/> set.</returns>
    protected virtual IStructDocNode NewNodeWithId(string id)
    {
        return new TParaNode
        {
            Id = id
        };
    }
}
