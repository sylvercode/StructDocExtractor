using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class EmphasesSerializer(ILogger<EmphasesSerializer>? logger = null)
    : BaseStyleSerializer<HtmlEmphases>(
        MarkdownStreamWriter.StyleState.Emphasis,
        logger)
{

}
