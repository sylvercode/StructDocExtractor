using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ListSerializer(ILogger<ListSerializer>? logger = null)
    : BaseBlockSerializer<HtmlList>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlList parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, nextChild, stream);
        stream.AddListCount();
    }

    protected override void OnAfterLastChildSerialize(HtmlList parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
    {
        base.OnAfterLastChildSerialize(parent, previousChild, stream);
        stream.RemoveListCount();
    }

}
