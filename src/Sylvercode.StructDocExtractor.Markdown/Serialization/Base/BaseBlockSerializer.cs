using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public class BaseBlockSerializer<THtmlNode, TChild>(ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, TChild>(logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    protected override void OnBeforeFirstChildSerialize(THtmlNode node, TChild fisrtChild, MarkdownStreamWriter stream)
    {
        if (!node.IsRoot)
            stream.StartParagraph();
    }

    protected override void OnAfterLastChildSerialize(THtmlNode node, TChild lastChild, MarkdownStreamWriter stream)
    {
        if (!node.IsRoot)
            stream.EnsureEndParagraphNext();
    }
}

public class BaseBlockSerializer<THtmlNode>(ILogger? logger = null)
    : BaseBlockSerializer<THtmlNode, IStructDocNode>(logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
