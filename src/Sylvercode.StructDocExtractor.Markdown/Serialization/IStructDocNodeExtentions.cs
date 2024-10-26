using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public static class IStructDocNodeExtentions
{
    public static bool IsAStyleNode(this IStructDocNode node)
        => node is HtmlEmphases || node is HtmlStrong;

    public static bool IsMultiStyleDelimiter(this IStructDocNode node, IStructDocNode? sibling)
        => sibling is null
            && node.Parent.IsAStyleNode();
}
