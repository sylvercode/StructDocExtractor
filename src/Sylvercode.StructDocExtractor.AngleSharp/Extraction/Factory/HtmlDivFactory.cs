using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;div&gt;</c> elements to <see cref="HtmlDiv"/> nodes.
/// </summary>
public class HtmlDivFactory() : BaseParagraphFactory<HtmlDiv>(TagNames.Div)
{
}
