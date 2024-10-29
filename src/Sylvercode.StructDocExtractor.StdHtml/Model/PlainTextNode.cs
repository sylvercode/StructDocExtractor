using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class PlainTextNode(string text, string id) :
    BaseStructDocNode<IStructDocNodeHolder>(id)
{
    public PlainTextNode(string text) : this(text, string.Empty)
    {
    }

    public string Text { get; protected set; } = text;
}
