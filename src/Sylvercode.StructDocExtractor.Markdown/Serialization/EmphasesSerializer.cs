using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class EmphasesSerializer(ILogger<EmphasesSerializer>? logger = null)
    : BaseStyleSerializer<HtmlEmphases>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlEmphases parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        stream.PushEmphasis();
    }

    protected override void OnAfterLastChildSerialize(HtmlEmphases parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
    {
        stream.PopEmphasis();
    }
}
