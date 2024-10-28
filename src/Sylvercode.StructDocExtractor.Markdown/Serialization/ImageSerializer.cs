using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ImageSerializer(ILogger<ImageSerializer>? logger = null)
    : BaseStructDocNodeSerializer<HtmlImg, MarkdownStreamWriter>(
        handler: new ImageHandler(),
        logger: logger)
{
    private sealed class ImageHandler()
        : IndentedSerializerHandler<HtmlImg, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Word))
    {
        public override void Serialize(HtmlImg obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            stream.Write($"![]({obj.Src})");
        }
    }
}
