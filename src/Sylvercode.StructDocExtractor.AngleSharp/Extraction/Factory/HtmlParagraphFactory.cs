using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlParagraphFactory() : BaseParagraphFactory<HtmlParagraph>(TagNames.P)
{
}
