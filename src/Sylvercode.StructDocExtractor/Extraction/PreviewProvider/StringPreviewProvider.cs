namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

public class StringPreviewProvider(int characterCountForPreview = 20) : IDataPreviewProvider<string>
{

    public string GetPreview(string data)
        => data.Length <= characterCountForPreview ? data : data[..characterCountForPreview];
}
