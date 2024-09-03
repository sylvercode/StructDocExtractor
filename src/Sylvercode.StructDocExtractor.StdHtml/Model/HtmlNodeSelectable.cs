namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public readonly struct HtmlNodeSelectable(string id, string[] styleClass, string tagName)
{
    public HtmlNodeSelectable() : this(id: "", styleClass: [], tagName: "") { }

    public string Id { get; } = id;
    public string[] StyleClass { get; } = styleClass;
    public string TagName { get; } = tagName;
}
