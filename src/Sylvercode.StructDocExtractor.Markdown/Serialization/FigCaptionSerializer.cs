using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits <see cref="HtmlFigCaption"/> content as an inline Markdown line within its parent figure block.</summary>
public class FigCaptionSerializer(ILogger<FigCaptionSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlFigCaption>(
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Line),
        logger: logger)
{

}
