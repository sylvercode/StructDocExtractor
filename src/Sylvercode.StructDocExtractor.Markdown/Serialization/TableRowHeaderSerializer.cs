using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlTableRowHeader"/> cell content within a Markdown GFM table header row.</summary>
public class TableRowHeaderSerializer(ILogger<TableRowHeaderSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableRowHeader>(logger: logger)
{

}
