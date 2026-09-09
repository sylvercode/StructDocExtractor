using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.AngleSharp;

/// <summary>
/// Entry-point provider that assembles the full default set of AngleSharp node factories and
/// exposes a configurable router selector for conditional extraction dispatch.
/// </summary>
/// <remarks>
/// Extends <see cref="HtmlNodeFactoryProvider{INode}"/> and implements
/// <see cref="IRouterExtractorSelectorProvider{INode}"/> to supply a <see cref="RouterSelector"/>
/// that determines which source nodes this provider handles. The selector can be built from a CSS
/// selector string or a custom <see cref="Func{TResult}"/> delegate. Subclasses may override
/// <see cref="HtmlHeadingFactoryOptions"/> to customise heading-level behaviour across the whole
/// provider.
/// </remarks>
public class AngleSharpNodeFactoryProvider : HtmlNodeFactoryProvider<INode>, IRouterExtractorSelectorProvider<INode>
{
    /// <summary>
    /// Matches incoming <see cref="INode"/> values against a delegate, used by the router to
    /// select this provider.
    /// </summary>
    protected sealed class RouterSelector(Func<INode, INode?> matchFunc) : IRouterExtractorSelector<INode>
    {
        /// <summary>
        /// Returns a match function that queries an element with the given CSS selector,
        /// or <see langword="null"/> if the node is not an element.
        /// </summary>
        /// <param name="elementSelector">A CSS selector string.</param>
        /// <returns>A function that returns the matched element or <see langword="null"/>.</returns>
        public static Func<INode, INode?> GetElementSelector(string elementSelector)
            => (node) => (node is IElement element) ? element.QuerySelector(elementSelector) : null;

        /// <summary>Initializes a <see cref="RouterSelector"/> that returns the node unchanged.</summary>
        public RouterSelector() : this((node) => node)
        {
        }

        /// <inheritdoc/>
        public INode? Match(INode data)
            => matchFunc(data);
    }

    /// <summary>Gets the router selector used to match this provider against incoming nodes.</summary>
    public IRouterExtractorSelector<INode> DefaultRouterSelector { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AngleSharpNodeFactoryProvider"/> using a CSS
    /// element selector to scope extraction to a sub-element.
    /// </summary>
    /// <param name="elementSelector">A CSS selector applied to the source element to find the root extraction node.</param>
    /// <param name="addDefaultFactory">Whether to register the full default set of HTML element factories.</param>
    public AngleSharpNodeFactoryProvider(string elementSelector, bool addDefaultFactory = true)
        : this(RouterSelector.GetElementSelector(elementSelector), addDefaultFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AngleSharpNodeFactoryProvider"/> with a custom
    /// node-matching function.
    /// </summary>
    /// <param name="selector">
    /// A function that returns the root extraction node for a given source node,
    /// or <see langword="null"/> to use an identity selector.
    /// </param>
    /// <param name="addDefaultFactory">Whether to register the full default set of HTML element factories.</param>
    public AngleSharpNodeFactoryProvider(Func<INode, INode?>? selector, bool addDefaultFactory = true)
    {
        DefaultRouterSelector = selector is not null ? new RouterSelector(selector) : new RouterSelector();
        if (addDefaultFactory)
            AddDefaultFactory();
    }

    /// <summary>
    /// Registers the complete default set of AngleSharp node factories (text, headings, paragraphs,
    /// lists, tables, inline elements, etc.) into this provider.
    /// </summary>
    protected void AddDefaultFactory()
    {
        AddFactory(new PlainTextNodeFactory());
        AddFactory(new HtmlHeadingFactory(HtmlHeadingFactoryOptions));
        AddFactory(new HtmlBrFactory());
        AddFactory(new HtmlParagraphFactory());
        AddFactory(new HtmlDivFactory());
        AddFactory(new HtmlFigureFactory());
        AddFactory(new HtmlAsideFactory());
        AddFactory(new HtmlEmphasesFactory());
        AddFactory(new HtmlStrongFactory());
        AddFactory(new HtmlAnchorFactory());
        AddFactory(new HtmlFigCaptionFactory());
        AddFactory(new HtmlImgFactory());
        AddFactory(new HtmlListFactory());
        AddFactory(new HtmlListItemFactory());
        AddFactory(new HtmlTableFactory());
        AddFactory(new HtmlTableHeaderFactory());
        AddFactory(new HtmlTableBodyFactory());
        AddFactory(new HtmlTableFooterFactory());
        AddFactory(new HtmlTableRowFactory());
        AddFactory(new HtmlTableRowDataFactory());
        AddFactory(new HtmlTableRowHeaderFactory());
    }

    /// <summary>
    /// Gets the optional <see cref="HtmlHeadingFactory.Options"/> passed to the heading factory;
    /// returns <see langword="null"/> to use the factory's own defaults.
    /// </summary>
    protected virtual HtmlHeadingFactory.Options? HtmlHeadingFactoryOptions => null;
}
