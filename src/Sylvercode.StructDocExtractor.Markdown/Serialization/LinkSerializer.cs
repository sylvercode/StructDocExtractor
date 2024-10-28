using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class LinkSerializer(ILogger<LinkSerializer>? logger = null) 
    : BaseMarkdownSerializer<HtmlAnchor>(
        handler: new LinkHandler(),
        logger: logger)
{
    public sealed class LinkHandler() : IndentedSerializerHandler<HtmlAnchor, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Word))
    {
        public override void Serialize(HtmlAnchor obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            if (!obj.ContentIsTextOnly)
            {
                WriteStandardLink(stream, obj.Href, obj.Href);
                return;
            }

            result.ContentSerialized = true;
            if (obj.Href.StartsWith("#^", StringComparison.InvariantCultureIgnoreCase))
                WriteWikiLink(stream, obj.Href, obj.TextContent);
            else if (obj.Href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                WriteStandardLink(stream, obj.Href, obj.TextContent);
            else if (Uri.TryCreate(obj.Href, UriKind.RelativeOrAbsolute, out Uri? hrefUri))
            {
                UriBuilder uriBuilder = new(hrefUri);
                if (uriBuilder.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    WriteStandardLink(stream, obj.Href, obj.TextContent);
                else
                    WriteWikiLink(stream, obj.Href, obj.TextContent);
            }
            else
                WriteWikiLink(stream, obj.Href, obj.TextContent);
        }

        private static void WriteStandardLink(MarkdownStreamWriter stream, string href, string text) => stream.Write($"[{text}]({href})");

        private static void WriteWikiLink(MarkdownStreamWriter stream, string href, string text)
        {
            if (href == text)
                stream.Write($"[[{href}]]");
            else
                stream.Write($"[[{href}|{text}]]");
        }
    }
}
