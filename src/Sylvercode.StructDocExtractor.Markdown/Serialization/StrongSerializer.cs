using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class StrongSerializer(ILogger<StrongSerializer>? logger = null)
    : BaseStyleSerializer<HtmlStrong>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlStrong parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        stream.PushStrong();
    }

    protected override void OnAfterLastChildSerialize(HtmlStrong parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
    {
        stream.PopStrong();
    }
}
