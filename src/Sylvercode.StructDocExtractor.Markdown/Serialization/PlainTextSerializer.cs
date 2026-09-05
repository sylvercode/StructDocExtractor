using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that writes <see cref="PlainTextNode"/> content verbatim to the Markdown output stream.</summary>
public class PlainTextSerializer(ILogger<PlainTextSerializer>? logger = null)
    : BaseStructDocNodeSerializer<PlainTextNode, MarkdownStreamWriter>(
        handler: new PlainTextHandler(),
        logger: logger)
{
    private sealed class PlainTextHandler : Handler
    {
        public override void Serialize(PlainTextNode obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            stream.Write(obj.Text);
        }
    }
}
