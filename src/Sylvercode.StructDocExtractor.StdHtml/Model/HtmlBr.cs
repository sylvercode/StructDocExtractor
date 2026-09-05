using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;br&gt;</c> line-break element, carrying no content.</summary>
public class HtmlBr() : BaseStructDocBlockWithAnyParentAndContent(string.Empty)
{
}
