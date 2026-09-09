using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlTableRowData"/> cell content within a Markdown GFM table row.</summary>
public class TableRowDataSerializer(ILogger<TableRowDataSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableRowData>(logger: logger)
{

}
