using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ImageSerializer(ILogger<ImageSerializer>? logger = null)
    : BaseStructDocNodeSerializer<HtmlImg, MarkdownStreamWriter>(logger)
{
    protected override void Serialize(HtmlImg obj, MarkdownStreamWriter stream, NodeSerializationResult result)
    {
        stream.Write($"![]({obj.Src})");
    }
}
