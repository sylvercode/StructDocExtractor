namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public readonly struct HtmlNodeDiscriminator(string id = "", string[]? styleClass = null, string tagName = "")
{
    public string Id { get; } = id;
    public string[] StyleClass { get; } = styleClass ?? [];
    public string TagName { get; } = tagName;
}
