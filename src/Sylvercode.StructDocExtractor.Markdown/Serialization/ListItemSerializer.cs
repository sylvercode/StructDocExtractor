using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ListItemSerializer(ILogger<ListItemSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlListItem>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlListItem parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, nextChild, stream);
        stream.StartLine();
        stream.Write("- ");
    }

}
