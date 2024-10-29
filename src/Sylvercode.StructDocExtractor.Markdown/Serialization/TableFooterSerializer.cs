using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableFooterSerializer(ILogger<TableFooterSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableFooter, HtmlTableRow>(logger: logger)
{

}
