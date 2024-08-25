namespace Sylvercode.DDBSrcModel.Html;

public readonly struct HtmlNodeSelectable(string id, string[] styleClass, string tagName)
{
    public HtmlNodeSelectable() : this(id: "", styleClass: [], tagName: "") { }

    public readonly string Id = id;
    public readonly string[] StyleClass = styleClass;
    public readonly string TagName = tagName;
}
