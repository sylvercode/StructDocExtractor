using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlDivFactory() : BaseParagraphFactory<HtmlDiv>(TagNames.Div)
{
}
