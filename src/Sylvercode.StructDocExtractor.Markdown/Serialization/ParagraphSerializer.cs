using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ParagraphSerializer(ILogger<ParagraphSerializer>? logger = null)
    : BaseBlockSerializer<HtmlParagraph>(logger)
{

}
