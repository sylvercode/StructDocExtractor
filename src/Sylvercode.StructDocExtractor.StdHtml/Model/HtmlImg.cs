using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlImg(string src, string id = "") :
    BaseStructDocNode(id),
    IStructDocReferencer
{
    public string Src { get; private set; } = src;

    #region IStructDocReferencer
    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.Embeded;

    public string GetReference() => Src;

    public void UpdateReference(string newReference) => Src = newReference;
    #endregion
}
