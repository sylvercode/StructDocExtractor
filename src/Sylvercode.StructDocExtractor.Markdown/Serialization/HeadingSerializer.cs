using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class HeadingSerializer(ILogger<HeadingSerializer>? logger = null)
    : BaseBlockSerializer<HtmlHeading>(logger)
{
    protected override void OnBeforeFirstChildSerialize(HtmlHeading parent, IStructDocNode nextChild, MarkdownStreamWriter stream)
    {
        base.OnBeforeFirstChildSerialize(parent, nextChild, stream);

        string headingStarter = new('#', parent.Level);
        stream.Write($"{headingStarter} ");
    }

    protected override void OnAfterLastChildSerialize(HtmlHeading parent, IStructDocNode previousChild, MarkdownStreamWriter stream)
    {
        base.OnAfterLastChildSerialize(parent, previousChild, stream);

        if (string.IsNullOrWhiteSpace(parent.Id))
            return;

        stream.StartWord();
        stream.Write($"^{parent.Id}");
    }
}
