using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public abstract class BaseStyleSerializer<THtmlNode>(
    MarkdownStreamWriter.StyleState style,
    ILogger<BaseStyleSerializer<THtmlNode>>? logger = null)
    : BaseMarkdownSerializer<THtmlNode>(
        holderHandler: new StyleHolderHandler(style),
        handler: new StyleHandler(),
        logger: logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{
    private sealed class StyleHandler : BaseMarkdownSerializer<THtmlNode>.Handler
    {
        public override void OnBeforeAsChildSerialize(THtmlNode node, IStructDocNode? previousNode, MarkdownStreamWriter stream)
        {
            if (node.HasContent
                && !node.IsMultiStyleDelimiter(previousNode))
                stream.StartWord();
        }

        public override void OnAfterAsChildSerialize(THtmlNode node, IStructDocNode? nextNode, MarkdownStreamWriter stream)
        {
            if (node.HasContent
                && !node.IsMultiStyleDelimiter(nextNode))
                stream.EnsureEndWordNext();
        }
    }

    private sealed class StyleHolderHandler(MarkdownStreamWriter.StyleState style) : BaseMarkdownSerializer<THtmlNode>.HolderHandler
    {
        public override void OnBeforeFirstChildSerialize(THtmlNode parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
        {
            stream.PushStyle(style);
        }

        public override void OnAfterLastChildSerialize(THtmlNode parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
        {
            stream.PopStyle(style);
        }
    }
}
