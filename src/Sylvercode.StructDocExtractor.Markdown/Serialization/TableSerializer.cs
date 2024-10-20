using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableSerializer(ILogger<TableSerializer>? logger = null)
    : BaseBlockSerializer<HtmlTable>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlTable parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, nextChild, stream);

        if (nextChild is HtmlTableHeader)
            return;

        if (nextChild is not HtmlTableRow row)
            return;

        if (row.Content.Count == 0)
            return;

        if (row.Content[0] is not HtmlTableRowHeader)
            return;

        string emptyHeader = string.Concat(Enumerable.Repeat("| ", row.Content.Count).Append("|"));
        stream.WriteLine(emptyHeader);
        WriteHeaderSeparater(stream, row.Content.Count);
    }

    public static void WriteHeaderSeparater(MarkdownStreamWriter stream, int columnCount)
    {
        stream.StartLine();
        string headerSeparater = string.Concat(Enumerable.Repeat("|-", columnCount).Append("|"));
        stream.WriteLine(headerSeparater);
    }

}
