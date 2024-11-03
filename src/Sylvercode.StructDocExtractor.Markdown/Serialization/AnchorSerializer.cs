using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class AnchorSerializer(ILogger<AnchorSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlAnchor>(
        handler: new LinkHandler(),
        logger: logger)
{
    public const string WikiScheme = "wiki";

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

            Uri hrefUri = new(obj.Href, UriKind.RelativeOrAbsolute);
            if (!hrefUri.IsAbsoluteUri)
            {
                WriteStandardLink(stream, obj.Href, obj.TextContent);
                return;
            }

            if (hrefUri.Scheme != WikiScheme)
            {
                WriteStandardLink(stream, obj.Href, obj.TextContent);
                return;
            }

            string wikiRef = WikiPath(hrefUri) + hrefUri.Fragment;
            WriteWikiLink(stream, wikiRef, obj.TextContent);
        }

        private static string WikiPath(Uri uri)
        {
            string localPath = uri.LocalPath;
            if (localPath == "/")
                return string.Empty;
            return localPath;
        }

        private static void WriteStandardLink(TextWriter stream, string href, string text)
            => stream.Write($"[{text}]({href})");

        private static void WriteWikiLink(TextWriter stream, string href, string text)
        {
            string unescapeHref = Uri.UnescapeDataString(href);
            if (unescapeHref == text)
                stream.Write($"[[{unescapeHref}]]");
            else
                stream.Write($"[[{unescapeHref}|{text}]]");
        }
    }
}
