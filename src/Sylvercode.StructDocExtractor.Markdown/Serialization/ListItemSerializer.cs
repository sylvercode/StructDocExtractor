using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlListItem"/> nodes as individual Markdown list item lines prefixed with <c>- </c>.</summary>
public class ListItemSerializer(ILogger<ListItemSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlListItem>(
        handler: new ListItemHandler(),
        logger: logger)
{
    private sealed class ListItemHandler()
        : IndentedSerializerHandler<HtmlListItem, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Line))
    {
        public override void Serialize(HtmlListItem obj, MarkdownStreamWriter stream, NodeSerializationResult result)
            => stream.Write("- ");
    }

}
