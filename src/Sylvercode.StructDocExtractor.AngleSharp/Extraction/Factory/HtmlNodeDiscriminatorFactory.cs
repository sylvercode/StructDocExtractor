using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

/// <summary>
/// Factory that creates a <see cref="HtmlNodeDiscriminator"/> from an AngleSharp <see cref="INode"/>.
/// </summary>
/// <remarks>
/// Non-whitespace text nodes are mapped to the <see cref="HtmlNodeDiscriminator.PlainTextTagName"/>
/// virtual tag. Non-element nodes that are not text nodes return an empty discriminator.
/// Element nodes are discriminated by their lower-cased tag name, id, and CSS class list.
/// </remarks>
public class HtmlNodeDiscriminatorFactory : IDataDiscriminatorFactory<INode, HtmlNodeDiscriminator>
{
    /// <summary>Creates a <see cref="HtmlNodeDiscriminator"/> that identifies the given DOM node.</summary>
    /// <param name="data">The AngleSharp DOM node to discriminate.</param>
    /// <returns>
    /// A <see cref="HtmlNodeDiscriminator"/> populated with the node's tag name, id, and classes,
    /// or an empty discriminator if the node type is not mappable.
    /// </returns>
    public HtmlNodeDiscriminator CreateDataDiscriminator(INode data)
    {
        if (data.NodeType is NodeType.Text
            && !string.IsNullOrWhiteSpace(data.TextContent))
            return new(tagName: HtmlNodeDiscriminator.PlainTextTagName);

        if (data is not IElement element)
            return new();

        return new(element.Id ?? "", [.. element.ClassList], element.TagName.ToLowerInvariant());
    }
}
