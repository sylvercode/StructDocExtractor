using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Abstract base factory for all AngleSharp node factories, converting AngleSharp
/// <see cref="INode"/> instances into structured document nodes via
/// <see cref="IHtmlNodeFactory{INode}"/>.
/// </summary>
/// <remarks>
/// Filters incoming nodes to <see cref="IElement"/> before delegating to
/// <see cref="BuildFromElement"/>. Subclasses implement <see cref="BuildFromElement"/>
/// to perform element-specific node creation; the base class handles result building
/// and error-result production via <see cref="AngleProcessTaskResultBuilder"/>.
/// </remarks>
public abstract class BaseAngleNodeFactory : IHtmlNodeFactory<INode>
{
    /// <summary>
    /// Gets the optional default selector criteria used to match this factory to a source element;
    /// returns <see langword="null"/> if no default is defined.
    /// </summary>
    public virtual List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; }

    /// <summary>
    /// Creates a new structural node from the given discriminator and source DOM node.
    /// </summary>
    /// <param name="discriminator">The <see cref="HtmlNodeDiscriminator"/> identifying the element.</param>
    /// <param name="data">The source AngleSharp DOM node.</param>
    /// <returns>
    /// An <see cref="IProcessTaskResult{TData,TDiscriminator}"/> containing the produced node,
    /// or an error result if the node could not be converted.
    /// </returns>
    public IProcessTaskResult<INode, HtmlNodeDiscriminator> NewNode(HtmlNodeDiscriminator discriminator, INode data)
    {
        AngleProcessTaskResultBuilder builder = new(data);
        builder.WithDataDiscriminator(discriminator);

        return BuildFromNode(builder, data) ? builder.Build() : ProcessTaskResult.NewError<INode, HtmlNodeDiscriminator>();
    }

    /// <summary>
    /// Builds the result from a raw <see cref="INode"/>, casting to <see cref="IElement"/> before
    /// calling <see cref="BuildFromElement"/>; returns <see langword="false"/> for non-element nodes
    /// by default.
    /// </summary>
    /// <param name="resultBuilder">The builder to populate with the extraction result.</param>
    /// <param name="node">The source DOM node.</param>
    /// <returns><see langword="true"/> if a node was produced; otherwise <see langword="false"/>.</returns>
    protected virtual bool BuildFromNode(AngleProcessTaskResultBuilder resultBuilder, INode node)
    {
        if (node is not IElement element)
            return false;

        return BuildFromElement(resultBuilder, element);
    }

    /// <summary>
    /// When overridden in a subclass, constructs the target structural node from the given element.
    /// </summary>
    /// <param name="resultBuilder">The builder to populate with the extraction result.</param>
    /// <param name="element">The source AngleSharp element.</param>
    /// <returns><see langword="true"/> if a node was produced; otherwise <see langword="false"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown by the base implementation.</exception>
    protected virtual bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement element)
    {
        throw new NotImplementedException();
    }
}
