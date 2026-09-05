using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

/// <summary>Abstract base for inline-style serializers that wrap node content in Markdown emphasis or strong markers using the <see cref="MarkdownStreamWriter"/> style stack.</summary>
/// <typeparam name="THtmlNode">The holder node type being serialized (typically <see cref="HtmlEmphases"/> or <see cref="HtmlStrong"/>).</typeparam>
/// <remarks>Pushes the configured style state before the first child is serialized and pops it after the last child, ensuring correct nesting and delimiter character selection.</remarks>
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
