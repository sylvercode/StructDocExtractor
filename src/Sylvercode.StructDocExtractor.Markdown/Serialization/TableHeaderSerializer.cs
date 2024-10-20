using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class TableHeaderSerializer(ILogger<TableHeaderSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlTableHeader, HtmlTableRow>(logger)
{
    protected override void OnAfterLastChildSerialize(HtmlTableHeader parent, HtmlTableRow previousChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, previousChild, stream);

        stream.StartLine();
        string headerSeparater = string.Concat(Enumerable.Repeat("|-", previousChild.Content.Count).Append("|"));
        stream.WriteLine(headerSeparater);
    }
}
