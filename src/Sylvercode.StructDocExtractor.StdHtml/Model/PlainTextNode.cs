using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing raw inline text content, wrapping a plain text string as a leaf structural node.</summary>
/// <param name="text">The raw text content for this node.</param>
/// <param name="id">The optional node identifier.</param>
public class PlainTextNode(string text, string id) :
    BaseStructDocNode<IStructDocNodeHolder>(id)
{
    /// <summary>Initializes a new instance of <see cref="PlainTextNode"/> with the specified text and an empty identifier.</summary>
    /// <param name="text">The raw text content for this node.</param>
    public PlainTextNode(string text) : this(text, string.Empty)
    {
    }

    /// <summary>Gets the raw text content of this node.</summary>
    public string Text { get; protected set; } = text;
}
