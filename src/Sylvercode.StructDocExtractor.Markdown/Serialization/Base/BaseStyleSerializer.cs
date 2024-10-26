using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public abstract class BaseStyleSerializer<THtmlNode>(ILogger<BaseStyleSerializer<THtmlNode>>? logger = null)
    : BaseMarkdownSerializer<THtmlNode>(logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{
    protected override void OnBeforeChildSerialize(THtmlNode node, IStructDocNode? previousNode, MarkdownStreamWriter stream)
    {
        if (node.HasContent
            && !node.IsMultiStyleDelimiter(previousNode))
            stream.StartWord();
    }

    protected override void OnAfterChildSerialize(THtmlNode node, IStructDocNode? nextNode, MarkdownStreamWriter stream)
    {
        if (node.HasContent
            && !node.IsMultiStyleDelimiter(nextNode))
            stream.EnsureEndWordNext();
    }
}
