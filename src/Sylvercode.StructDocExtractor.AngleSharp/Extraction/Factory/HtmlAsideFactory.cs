using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;aside&gt;</c> elements to <see cref="HtmlAside"/> nodes.
/// </summary>
public class HtmlAsideFactory() : BaseParagraphFactory<HtmlAside>(TagNames.Aside)
{
}
