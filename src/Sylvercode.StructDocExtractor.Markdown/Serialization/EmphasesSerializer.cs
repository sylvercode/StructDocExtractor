using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class EmphasesSerializer : BaseStructDocNodeHolderSerializer<HtmlEmphases, MarkdownStreamWriter, IStructDocNode>
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
