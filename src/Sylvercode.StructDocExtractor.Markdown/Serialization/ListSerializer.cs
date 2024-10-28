using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class ListSerializer(ILogger<ListSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlList>(
        holderHandler: new ListHolderHandler(),
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Paragraph),
        logger: logger)
{
    private sealed class ListHolderHandler : BaseMarkdownSerializer<HtmlList>.HolderHandler
    {
        public override void OnBeforeFirstChildSerialize(HtmlList parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
            => stream.AddListCount();

        public override void OnAfterLastChildSerialize(HtmlList parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
            => stream.RemoveListCount();
    }

}
