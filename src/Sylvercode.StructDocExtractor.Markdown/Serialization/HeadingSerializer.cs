using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class HeadingSerializer(ILogger<HeadingSerializer>? logger = null)
    : BaseMarkdownSerializer<HtmlHeading>(
        holderHandler: new HeadingHolderHandler(),
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Paragraph),
        logger: logger)
{
    private sealed class HeadingHolderHandler : HolderHandler
    {
        public override void OnBeforeFirstChildSerialize(HtmlHeading parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
        {
            string headingStarter = new('#', parent.Level);
            stream.Write($"{headingStarter} ");
        }

        public override void OnAfterLastChildSerialize(HtmlHeading parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
        {
            if (string.IsNullOrWhiteSpace(parent.Id))
                return;

            stream.StartWord();
            stream.Write($"^{parent.Id}");
        }
    }
}
