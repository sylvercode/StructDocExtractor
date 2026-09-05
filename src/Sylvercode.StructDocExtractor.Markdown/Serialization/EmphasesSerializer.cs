using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that wraps <see cref="HtmlEmphases"/> content in Markdown italic markers via the <see cref="MarkdownStreamWriter"/> style stack.</summary>
public class EmphasesSerializer(ILogger<EmphasesSerializer>? logger = null)
    : BaseStyleSerializer<HtmlEmphases>(
        MarkdownStreamWriter.StyleState.Emphasis,
        logger)
{

}
