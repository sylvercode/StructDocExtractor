using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableRowSerializer(ILogger<TableRowSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableRow, IHtmlTableRowElement>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlTableRow parent, IHtmlTableRowElement nextChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, nextChild, stream);
        stream.StartLine();
        stream.Write("| ");
    }

    protected override void OnBetweenSiblingSerialize(HtmlTableRow parent, IHtmlTableRowElement previousChild, IHtmlTableRowElement nextChild, MarkdownStreamWriter stream)
    {
        base.OnBetweenChildrenSerialize(parent, previousChild, nextChild, stream);

        stream.Write(" | ");
    }

    protected override void OnAfterLastChildSerialize(HtmlTableRow parent, IHtmlTableRowElement previousChild, MarkdownStreamWriter stream)
    {
        base.OnAfterLastChildSerialize(parent, previousChild, stream);

        stream.Write(" |");

        if (parent.Parent is HtmlTable
            && previousChild is HtmlTableRowHeader)
            TableSerializer.WriteHeaderSeparater(stream, parent.Content.Count);
    }
}
