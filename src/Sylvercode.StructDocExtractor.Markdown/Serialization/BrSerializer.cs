using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlBr"/> nodes as an HTML <c>&lt;br&gt;</c> inline element, preserving explicit line breaks in Markdown output.</summary>
public class BrSerializer(ILogger<ImageSerializer>? logger = null)
    : BaseStructDocNodeSerializer<HtmlBr, MarkdownStreamWriter>(
        handler: new ImageHandler(),
        logger: logger)
{
    private sealed class ImageHandler()
        : IndentedSerializerHandler<HtmlBr, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.None))
    {
        public override void Serialize(HtmlBr obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            stream.Write("<br>");
        }
    }
}
