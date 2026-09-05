using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits the <see cref="HtmlTableFooter"/> section by delegating to child <see cref="HtmlTableRow"/> serializers.</summary>
public class TableFooterSerializer(ILogger<TableFooterSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableFooter, HtmlTableRow>(logger: logger)
{

}
