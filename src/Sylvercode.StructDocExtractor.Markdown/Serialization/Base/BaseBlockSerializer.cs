using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public class BaseBlockSerializer<THtmlNode, TChild>(ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, TChild>(logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    protected override void OnBeforeChildSerialize(THtmlNode node, THtmlNode? previousNode, MarkdownStreamWriter stream)
    {
        stream.EndLineIfStarted();
        stream.WriteLine();
    }

    protected override void OnAfterChildSerialize(THtmlNode node, THtmlNode? nextNode, MarkdownStreamWriter stream)
    {
        stream.EndLineIfStarted();
        stream.WriteLine();
    }
}

public class BaseBlockSerializer<THtmlNode>(ILogger? logger = null)
    : BaseBlockSerializer<THtmlNode, IStructDocNode>(logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
