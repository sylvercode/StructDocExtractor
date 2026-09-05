using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that outputs <see cref="HtmlList"/> nodes as Markdown bulleted or numbered lists, managing list nesting depth and spacing via the <see cref="MarkdownStreamWriter"/> list counter.</summary>
public class ListSerializer(ILogger<ListSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlList>(
        handler: new ListHandler(),
        logger: logger)
{
    private sealed class ListHandler : Handler
    {
        public override void OnBeforeAsChildSerialize(HtmlList node, IStructDocNode? previousNode, MarkdownStreamWriter stream)
        {
            if (node.Content.Count == 0)
                return;

            stream.AddListCount();
            if (stream.IsInSubList)
                stream.Indent();
            stream.DoSpaceOperation(NextSpaceOperation(stream.IsInSubList));
        }

        public override void OnAfterAsChildSerialize(HtmlList node, IStructDocNode? nextNode, MarkdownStreamWriter stream)
        {
            if (node.Content.Count == 0)
                return;

            stream.EnsureSpaceOperation(NextSpaceOperation(stream.IsInSubList));
            if (stream.IsInSubList)
                stream.Unindent();
            stream.RemoveListCount();
        }

        private static IndentedStreamWriter.SpaceOperationType NextSpaceOperation(bool isInSubList)
            => isInSubList
                ? IndentedStreamWriter.SpaceOperationType.Line
                : IndentedStreamWriter.SpaceOperationType.Paragraph;
    }
}
