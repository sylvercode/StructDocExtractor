using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlDataPreviewProvider : IDataPreviewProvider<IElement>
{
    public string GetPreview(IElement data)
    {
        throw new NotImplementedException();
    }
}
