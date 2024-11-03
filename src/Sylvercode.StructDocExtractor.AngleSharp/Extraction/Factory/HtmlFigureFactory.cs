using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlFigureFactory() : BaseParagraphFactory<HtmlFigure>(TagNames.Figure)
{
}
