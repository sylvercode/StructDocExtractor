using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class PlainTextSerializer(ILogger<PlainTextSerializer>? logger = null)
    : BaseStructDocNodeSerializer<PlainTextNode, MarkdownStreamWriter>(logger)
{
    protected override void Serialize(PlainTextNode obj, MarkdownStreamWriter stream, NodeSerializationResult result)
    {
        stream.Write(obj.Text);
    }
}
