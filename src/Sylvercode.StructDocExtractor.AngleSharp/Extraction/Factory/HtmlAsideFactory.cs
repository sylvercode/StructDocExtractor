using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlAsideFactory() : BaseParagraphFactory<HtmlAside>(TagNames.Aside)
{
}
