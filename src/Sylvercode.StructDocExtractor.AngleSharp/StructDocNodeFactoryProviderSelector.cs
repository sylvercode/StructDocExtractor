using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp;

/// <summary>
/// Strategy dispatcher that selects the appropriate
/// <see cref="IStructDocNodeFactoryProvider{TData,TDiscriminator}"/> for a given
/// AngleSharp <see cref="INode"/> by evaluating registered selector predicates in order.
/// </summary>
/// <remarks>
/// Selectors are tested in insertion order; the first matching selector's provider is returned.
/// If no selector accepts the node, <see cref="GetFactoryProvider"/> throws
/// <see cref="InvalidOperationException"/>. Use <see cref="Add"/> to register selector-provider
/// pairs before dispatching.
/// </remarks>
public class StructDocNodeFactoryProviderSelector
{
    /// <summary>
    /// Contract for a selector that tests whether a given <see cref="INode"/> should be handled
    /// by an associated factory provider, and returns the root extraction node when valid.
    /// </summary>
    public interface ISelector
    {
        /// <summary>
        /// Determines whether this selector accepts <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The source node to test.</param>
        /// <param name="rootExtraction">
        /// When this method returns <see langword="true"/>, contains the node to use as the
        /// extraction root; otherwise undefined.
        /// </param>
        /// <returns><see langword="true"/> if the node is accepted; otherwise <see langword="false"/>.</returns>
        bool IsValid(INode node, out INode rootExtraction);
    }

    private class Entry(ISelector selector, IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> factoryProvider)
    {
        public ISelector Selector { get; } = selector;
        public IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> FactoryProvider { get; } = factoryProvider;
    }

    private readonly List<Entry> _FactoryProviders = [];

    /// <summary>
    /// Registers a selector-provider pair in this dispatcher.
    /// </summary>
    /// <param name="selector">The selector that decides whether the provider handles a node.</param>
    /// <param name="factoryProvider">The factory provider to return when the selector matches.</param>
    public void Add(ISelector selector, IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> factoryProvider)
        => _FactoryProviders.Add(new Entry(selector, factoryProvider));

    /// <summary>
    /// Returns the factory provider for the first selector that accepts <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The source node to dispatch.</param>
    /// <param name="rootExtraction">
    /// When this method returns, contains the extraction root node determined by the matched selector.
    /// </param>
    /// <returns>The <see cref="IStructDocNodeFactoryProvider{TData,TDiscriminator}"/> for the matched selector.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no registered selector accepts <paramref name="node"/>.
    /// </exception>
    public IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> GetFactoryProvider(
        INode node,
        out INode rootExtraction)
    {
        foreach (var entry in _FactoryProviders)
        {
            if (entry.Selector.IsValid(node, out rootExtraction))
                return entry.FactoryProvider;
        }

        throw new InvalidOperationException("No factory provider found");
    }
}
