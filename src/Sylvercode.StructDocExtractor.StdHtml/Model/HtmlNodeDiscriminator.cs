namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Value type carrying the HTML tag name, element ID, and CSS classes used to discriminate factory selection for HTML nodes.</summary>
/// <param name="id">The HTML element id attribute value.</param>
/// <param name="styleClass">The CSS class names applied to the element.</param>
/// <param name="tagName">The HTML tag name of the element (e.g., <c>div</c>, <c>p</c>).</param>
public class HtmlNodeDiscriminator(string id = "", string[]? styleClass = null, string tagName = "")
{
    /// <summary>Sentinel tag name used to identify plain-text pseudo-nodes in the discriminator.</summary>
    public const string PlainTextTagName = "|PlainText|";

    /// <summary>Gets the HTML element id attribute value.</summary>
    public string Id { get; } = id;
    /// <summary>Gets the CSS class names applied to the HTML element.</summary>
    public string[] StyleClass { get; } = styleClass ?? [];
    /// <summary>Gets the HTML tag name of the element (e.g., <c>div</c>, <c>p</c>).</summary>
    public string TagName { get; } = tagName;

    /// <summary>Returns a combined discriminator string in <c>tagName#id.class1.class2</c> format.</summary>
    /// <returns>A string combining tag name, id, and class names for diagnostic and matching purposes.</returns>
    public override string ToString()
    {
        return $"{TagName}#{Id}.{string.Join(".", StyleClass)}";
    }
}
