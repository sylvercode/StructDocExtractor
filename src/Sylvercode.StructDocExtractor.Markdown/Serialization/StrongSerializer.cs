using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class StrongSerializer(ILogger<StrongSerializer>? logger = null)
    : BaseStyleSerializer<HtmlStrong>(
        MarkdownStreamWriter.StyleState.Strong,
        logger)
{
}
