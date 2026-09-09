using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that wraps <see cref="HtmlStrong"/> content in Markdown bold markers via the <see cref="MarkdownStreamWriter"/> style stack.</summary>
public class StrongSerializer(ILogger<StrongSerializer>? logger = null)
    : BaseStyleSerializer<HtmlStrong>(
        MarkdownStreamWriter.StyleState.Strong,
        logger)
{
}
