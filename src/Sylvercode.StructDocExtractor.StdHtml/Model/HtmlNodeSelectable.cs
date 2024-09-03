namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public readonly struct HtmlNodeDiscriminator(string id, string[] styleClass, string tagName)
{
    public HtmlNodeDiscriminator() : this(id: "", styleClass: [], tagName: "") { }

    public string Id { get; } = id;
    public string[] StyleClass { get; } = styleClass;
    public string TagName { get; } = tagName;
}
