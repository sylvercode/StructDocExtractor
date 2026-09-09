using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Depth-aware stack of <see cref="HtmlNodeDiscriminator"/> entries and factory providers maintained during recursive HTML extraction.</summary>
/// <typeparam name="TExtractionData">The type of raw extraction data (typically an AngleSharp DOM element).</typeparam>
/// <remarks>
/// Implements <see cref="IStructDocNodeFactoryProviderStack{TExtractionData,TDiscriminator}"/> to enable layered factory
/// resolution with fallback to the injected default provider.  Each stack entry associates an optional
/// factory provider override with the discriminator and source node at that recursion depth, so
/// <see cref="GetActiveSrcNodeFactoryProvider"/> can walk inward until it finds the nearest active provider.
/// The stack can be projected into an <see cref="IStructDataStack{TData}"/> for score-based node selection
/// or into an <see cref="IStructDocNodeStack"/> for source-node traversal.
/// </remarks>
public class HtmlExtractionStack<TExtractionData>(
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> defaultSrcNodeFactoryProvider)
    : IStructDocNodeFactoryProviderStack<TExtractionData, HtmlNodeDiscriminator>
{
    /// <summary>Represents a single level of the extraction stack, capturing the HTML node context at that recursion depth.</summary>
    public class Entry
    {
        /// <summary>Gets the recursion depth at which this entry was pushed.</summary>
        public int Depth { get; private set; }
        /// <summary>Gets the <see cref="HtmlNodeDiscriminator"/> for the HTML element at this stack level, or <see langword="null"/> if not set.</summary>
        public HtmlNodeDiscriminator? NodeDiscriminator { get; private set; }
        /// <summary>Gets the extracted <see cref="IStructDocNode"/> produced at this stack level, or <see langword="null"/> if not yet available.</summary>
        public IStructDocNode? SrcNode { get; private set; }
        /// <summary>Gets the factory provider override active at this stack level, or <see langword="null"/> to defer to the parent level.</summary>
        public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator>? SrcNodeFactoryProvider { get; private set; }
    }

    private readonly Stack<Entry> m_Stack = [];

    /// <summary>Gets the default factory provider used when no stack entry provides an override.</summary>
    public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> DefaultSrcNodeFactoryProvider => defaultSrcNodeFactoryProvider;

    /// <summary>Returns the current stack contents projected as an <see cref="IStructDataStack{TData}"/> of <see cref="HtmlNodeDiscriminator"/> values for score-based node selection.</summary>
    /// <returns>A struct data stack view containing the discriminators of all entries that have a non-null discriminator.</returns>
    public IStructDataStack<HtmlNodeDiscriminator> AsStructDataStack()
    {
        return new BaseStructDataStack<HtmlNodeDiscriminator>(
            from entry in m_Stack
            let node = entry.NodeDiscriminator
            where node is not null
            select node
        );
    }

    /// <summary>Returns the current stack contents projected as an <see cref="IStructDocNodeStack"/> of source nodes for tree traversal.</summary>
    /// <returns>A source-node stack view containing the <see cref="IStructDocNode"/> values of all entries that have a non-null source node.</returns>
    public IStructDocNodeStack AsSrcNodeStack()
    {
        return new StructDocNodeStack(
            from entry in m_Stack
            let node = entry.SrcNode
            where node is not null
            select node
        );
    }

    /// <summary>Returns the innermost active factory provider by walking the stack, falling back to <see cref="DefaultSrcNodeFactoryProvider"/> if no entry has one.</summary>
    /// <returns>The nearest <see cref="IStructDocNodeFactoryProvider{TExtractionData,TDiscriminator}"/> found on the stack, or the default provider.</returns>
    public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> GetActiveSrcNodeFactoryProvider() =>
        m_Stack.First(entry => entry.SrcNodeFactoryProvider is not null)?.SrcNodeFactoryProvider ?? DefaultSrcNodeFactoryProvider;
}
