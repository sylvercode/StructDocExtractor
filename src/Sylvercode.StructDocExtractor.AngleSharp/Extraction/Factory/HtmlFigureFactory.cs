using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that converts AngleSharp <c>&lt;figure&gt;</c> elements to <see cref="HtmlFigure"/> nodes.
/// </summary>
public class HtmlFigureFactory() : BaseParagraphFactory<HtmlFigure>(TagNames.Figure)
{
}
