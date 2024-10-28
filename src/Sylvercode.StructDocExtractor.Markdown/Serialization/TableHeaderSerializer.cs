using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableHeaderSerializer(ILogger<TableHeaderSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableHeader, HtmlTableRow>(
        holderHandler: new TableHeaderHolderHandler(),
        logger: logger)
{
    private sealed class TableHeaderHolderHandler : HolderHandler
    {
        public override void OnAfterLastChildSerialize(HtmlTableHeader parent, HtmlTableRow previousChild, MarkdownStreamWriter stream)
        {
            TableSerializer.WriteHeaderSeparater(stream, previousChild.Content.Count);
        }
    }
}
