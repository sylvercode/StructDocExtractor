using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.Model;

/// <summary>Concrete root block node that serves as the top-level container of a structured document tree.</summary>
/// <param name="id">The optional identifier for this root block; defaults to an empty string.</param>
public class StructDocRootBlock(string id = "") :
    BaseStructDocRootBlockWithAnyContent(id)
{

}
