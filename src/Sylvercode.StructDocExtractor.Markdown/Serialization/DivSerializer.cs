using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class DivSerializer : BaseStructDocNodeHolderSerializer<HtmlDiv, MarkdownStreamWriter, IStructDocNode>
{
    protected override void OnBeforeChildSerialize(HtmlDiv node, HtmlDiv? previousNode, MarkdownStreamWriter stream)
    {
        stream.EndLineIfStarted();
        stream.WriteLine();
    }

    protected override void OnAfterChildSerialize(HtmlDiv node, HtmlDiv? nextNode, MarkdownStreamWriter stream)
    {
        stream.EndLineIfStarted();
        stream.WriteLine();
    }
}
