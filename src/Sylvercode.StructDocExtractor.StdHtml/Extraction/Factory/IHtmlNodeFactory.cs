using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

/// <summary>
/// Contract for HTML-specific node factories keyed on <see cref="HtmlNodeDiscriminator"/>, extending
/// <see cref="IStructDocNodeFactory{TDataDiscriminator,TStackDataDiscriminator}"/> with a default selector set.
/// </summary>
/// <typeparam name="TDataDiscriminator">The type of source element this factory discriminates against.</typeparam>
public interface IHtmlNodeFactory<TDataDiscriminator> : IStructDocNodeFactory<TDataDiscriminator, HtmlNodeDiscriminator>
{
    /// <summary>Gets the default score-criteria sets used to select this factory when no explicit selector is provided.</summary>
    List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; }
}
