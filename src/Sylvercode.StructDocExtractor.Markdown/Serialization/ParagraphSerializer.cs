using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ParagraphSerializer(ILogger<ParagraphSerializer>? logger = null)
    : BaseMarkdownSerializer<IStructDocNodeHolder<IStructDocNode>>(
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Paragraph),
        logger: logger),
    IStructDocNodeSerializerServiceInit
{
    IEnumerable<Type> IStructDocNodeSerializerServiceInit.GetDefaultSeriazableType()
    {
        yield return typeof(HtmlParagraph);
        yield return typeof(HtmlDiv);
        yield return typeof(HtmlFigure);
        yield return typeof(HtmlAside);
    }
}
