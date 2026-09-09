using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlAnchor"/> nodes as Markdown inline links or wiki-links, using <see cref="MarkdownLinkFormater"/> for formatting.</summary>
public class AnchorSerializer(ILogger<AnchorSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlAnchor>(
        handler: new LinkHandler(),
        logger: logger)
{
    /// <summary>Handler that formats anchor content as a Markdown link, writing the full link when content is text-only or just the href otherwise.</summary>
    public sealed class LinkHandler() : IndentedSerializerHandler<HtmlAnchor, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.None))
    {
        private static readonly MarkdownLinkFormater _linkFormater = new(useWikilink: true);

        /// <inheritdoc/>
        public override void Serialize(HtmlAnchor obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            if (!obj.HasContent)
                return;

            if (obj.ContentIsTextOnly)
            {
                result.ContentSerialized = true;
                stream.Write(_linkFormater.Format(obj.Href, obj.TextContent));
            }
            else
                stream.Write(MarkdownLinkFormater.FormatStandardLink(obj.Href));
        }

    }
}
