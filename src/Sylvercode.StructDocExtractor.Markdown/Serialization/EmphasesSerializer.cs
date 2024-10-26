using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class EmphasesSerializer(ILogger<EmphasesSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlEmphases>(logger)
{
    protected override void OnBeforeChildSerialize(HtmlEmphases node, HtmlEmphases? previousNode, MarkdownStreamWriter stream)
    {
        if (node.HasContent
            && !node.IsMultiStyleDelimiter(previousNode))
            stream.StartWord();
    }
    protected override void OnBeforeFirstChildSerialize(HtmlEmphases parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        stream.PushEmphasis();
    }

    protected override void OnAfterLastChildSerialize(HtmlEmphases parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
    {
        stream.PopEmphasis();
    }

    protected override void OnAfterChildSerialize(HtmlEmphases node, HtmlEmphases? nextNode, MarkdownStreamWriter stream)
    {
        if (node.HasContent
            && !node.IsMultiStyleDelimiter(nextNode))
            stream.EnsureEndWordNext();
    }
}
