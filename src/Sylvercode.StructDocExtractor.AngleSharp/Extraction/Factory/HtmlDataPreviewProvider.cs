using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Preview provider that extracts a short human-readable summary from an AngleSharp <see cref="INode"/>.
/// </summary>
/// <remarks>
/// Text content is capped at 50 characters. Element nodes are prefixed with their tag name;
/// plain text nodes are prefixed with <c>|PlainText|</c>. A <see langword="null"/> data value
/// returns <see cref="NullPreview"/>.
/// </remarks>
public class HtmlDataPreviewProvider : IDataPreviewProvider<INode>
{
    /// <summary>The preview string returned when the source data is <see langword="null"/>.</summary>
    public const string NullPreview = "<<null>>";

    /// <summary>Returns a short preview string for the given AngleSharp DOM node.</summary>
    /// <param name="data">The DOM node to preview, or <see langword="null"/>.</param>
    /// <returns>A truncated text preview prefixed with the node's tag or type.</returns>
    public string GetPreview(INode? data)
    {
        if (data is null)
            return NullPreview;

        string textContentPreview = data.TextContent.Length <= 50
            ? data.TextContent
            : data.TextContent[..50];

        if (data is not IElement element)
            return $"|PlainText| {textContentPreview}";

        return $"<{element.TagName}> {textContentPreview}";
    }
}
