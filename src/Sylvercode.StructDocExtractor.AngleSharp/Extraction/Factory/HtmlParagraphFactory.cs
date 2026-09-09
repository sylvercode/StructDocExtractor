using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;p&gt;</c> elements to <see cref="HtmlParagraph"/> nodes.
/// </summary>
public class HtmlParagraphFactory() : BaseParagraphFactory<HtmlParagraph>(TagNames.P)
{
}
