using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Provides extension methods on <see cref="IStructDocNode"/> for Markdown-specific inline style context detection.</summary>
public static class IStructDocNodeExtentions
{
    /// <summary>Returns a value indicating whether <paramref name="node"/> is an inline style node (emphasis or strong).</summary>
    /// <param name="node">The node to test.</param>
    /// <returns><see langword="true"/> if <paramref name="node"/> is <see cref="HtmlEmphases"/> or <see cref="HtmlStrong"/>; otherwise <see langword="false"/>.</returns>
    public static bool IsAStyleNode(this IStructDocNode node)
        => node is HtmlEmphases || node is HtmlStrong;

    /// <summary>Returns a value indicating whether <paramref name="node"/> is a boundary delimiter between adjacent style nodes, used to suppress redundant word-boundary spacing.</summary>
    /// <param name="node">The style node being serialized.</param>
    /// <param name="sibling">The adjacent sibling node, or <see langword="null"/> if there is none.</param>
    /// <returns><see langword="true"/> when <paramref name="sibling"/> is <see langword="null"/> and the node's parent is itself a style node.</returns>
    public static bool IsMultiStyleDelimiter(this IStructDocNode node, IStructDocNode? sibling)
        => sibling is null
            && node.Parent.IsAStyleNode();
}
