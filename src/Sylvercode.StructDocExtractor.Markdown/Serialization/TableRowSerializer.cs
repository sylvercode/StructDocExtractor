using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableRowSerializer(ILogger<TableRowSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableRow, IHtmlTableRowElement>(
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Line),
        holderHandler: new TableRowHolderHandler(),
        logger: logger)
{
    private sealed class TableRowHolderHandler : HolderHandler
    {
        public override void OnBeforeFirstChildSerialize(HtmlTableRow parent, IHtmlTableRowElement nextChild, MarkdownStreamWriter stream)
        {
            stream.Write("| ");
        }

        public override void OnBetweenSiblingSerialize(HtmlTableRow parent, IHtmlTableRowElement previousChild, IHtmlTableRowElement nextChild, MarkdownStreamWriter stream)
        {
            stream.Write(" | ");
        }

        public override void OnAfterLastChildSerialize(HtmlTableRow parent, IHtmlTableRowElement lastChild, MarkdownStreamWriter stream)
        {
            stream.Write(" |");

            if (parent.Parent is HtmlTable
                && lastChild is HtmlTableRowHeader)
                TableSerializer.WriteHeaderSeparater(stream, parent.Content.Count);
        }
    }
}
