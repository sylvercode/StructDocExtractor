using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

/// <summary>
/// Concrete factory provider for HTML node factories, extending <see cref="StructDocNodeFactoryProvider{TExtractionData,TStackDataDiscriminator}"/>
/// to enforce that each registered factory carries a <see cref="IHtmlNodeFactory{TDataDiscriminator}.DefaultSelector"/>.
/// </summary>
/// <remarks>
/// Serves as the core routing mechanism for the HTML extraction pipeline. Each registered
/// <see cref="IHtmlNodeFactory{TExtractionData}"/> is paired with a <see cref="StackScoreCalculator{T}"/>
/// built from its <c>DefaultSelector</c>, allowing the provider to choose the most contextually
/// appropriate factory by scoring the current <see cref="HtmlNodeDiscriminator"/> traversal stack.
/// </remarks>
/// <typeparam name="TExtractionData">The type of source element supplied to the registered factories.</typeparam>
public class HtmlNodeFactoryProvider<TExtractionData> : StructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator>
{
    /// <summary>
    /// Registers an <see cref="IHtmlNodeFactory{TDataDiscriminator}"/> using its own
    /// <see cref="IHtmlNodeFactory{TDataDiscriminator}.DefaultSelector"/> as the score calculator.
    /// </summary>
    /// <param name="factory">The HTML node factory to register; must have a non-null <c>DefaultSelector</c>.</param>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="factory"/> has a null <c>DefaultSelector</c>.</exception>
    public void AddFactory(IHtmlNodeFactory<TExtractionData> factory)
    {
        if (factory.DefaultSelector is null)
            throw new InvalidOperationException("No selector set in factory");
        AddFactory(factory, new StackScoreCalculator<HtmlNodeDiscriminator>(factory.DefaultSelector));
    }
}
