using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that wraps block-level container nodes (<see cref="HtmlParagraph"/>, <see cref="HtmlDiv"/>, <see cref="HtmlFigure"/>, <see cref="HtmlAside"/>) in a Markdown paragraph block.</summary>
public class ParagraphSerializer(ILogger<ParagraphSerializer>? logger = null)
    : BaseMarkdownSerializer<IStructDocNodeHolder<IStructDocNode>>(
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Paragraph),
        logger: logger),
    IStructDocNodeSerializerServiceInit
{
    /// <inheritdoc/>
    IEnumerable<Type> IStructDocNodeSerializerServiceInit.GetDefaultSeriazableType()
    {
        yield return typeof(HtmlParagraph);
        yield return typeof(HtmlDiv);
        yield return typeof(HtmlFigure);
        yield return typeof(HtmlAside);
    }
}
