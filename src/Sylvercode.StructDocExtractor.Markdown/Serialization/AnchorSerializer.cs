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
    public sealed class LinkHandler() : IndentedSerializerHandler<HtmlAnchor, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Word))
    {
        private static readonly MarkdownLinkFormater _linkFormater = new(useWikilink: true);

        public override void Serialize(HtmlAnchor obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
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
