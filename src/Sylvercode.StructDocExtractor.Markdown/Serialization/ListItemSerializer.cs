using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlListItem"/> nodes as individual Markdown list item lines prefixed with <c>- </c>.</summary>
public class ListItemSerializer(ILogger<ListItemSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlListItem>(
        holderHandler: new ListItemHolderHandler(),
        handler: new ListItemHandler(),
        logger: logger)
{
    private sealed class ListItemHolderHandler : HolderHandler
    {
        public override void OnAfterLastChildSerialize(HtmlListItem parent, IStructDocNode lastChild, MarkdownStreamWriter stream)
        {
            if (string.IsNullOrEmpty(parent.Id))
                return;

            stream.StartLine();
            stream.Indent();
            stream.Write($"^{parent.Id}");
            stream.Unindent();
        }
    }

    private sealed class ListItemHandler()
        : IndentedSerializerHandler<HtmlListItem, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Line))
    {
        public override void Serialize(HtmlListItem obj, MarkdownStreamWriter stream, NodeSerializationResult result)
            => stream.Write("- ");
    }

}
