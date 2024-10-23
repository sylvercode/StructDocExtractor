using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public class BaseBlockSerializer<THtmlNode, TChild>(ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, TChild>(logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    protected override void OnBeforeFirstChildSerialize(THtmlNode node, TChild nextChild, MarkdownStreamWriter stream)
        => stream.StartParagraph();
}

public class BaseBlockSerializer<THtmlNode>(ILogger? logger = null)
    : BaseBlockSerializer<THtmlNode, IStructDocNode>(logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
